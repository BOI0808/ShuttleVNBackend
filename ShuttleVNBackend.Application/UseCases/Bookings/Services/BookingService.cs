using System.Security.Cryptography;
using System.Text;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Bookings;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;
using ShuttleVNBackend.Application.Interfaces.Repositories.Users;
using ShuttleVNBackend.Application.Interfaces.Users;
using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Courts;
using ShuttleVNBackend.Core.Entities.System.Enums;
using ShuttleVNBackend.Core.Entities.Users;
using ShuttleVNBackend.Core.Entities.Users.Enums;
using ValidationException = ShuttleVNBackend.Application.Exceptions.ValidationException;

namespace ShuttleVNBackend.Application.UseCases.Bookings.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    ICustomerRepository customerRepository,
    IVerificationCodeRepository verificationCodeRepository,
    ICurrentUser currentUser,
    TimeProvider clock,
    IUnitOfWork unitOfWork)
{
    private static readonly TimeZoneInfo VietnamTz =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

    public async Task<PricePreviewResponse> GetPricePreviewAsync(
        PricePreviewRequest request, CancellationToken ct = default)
    {
        ValidateTimeSlot(request.StartTime, request.EndTime);

        var schedule = await bookingRepository.GetScheduleAsync(
            request.CourtId, GetIsoDayOfWeek(request.Date), ct);

        ValidateSchedule(schedule, request.StartTime, request.EndTime);

        var rules = await bookingRepository.GetPricingRulesAsync(
            request.CourtId, GetIsoDayOfWeek(request.Date), ct);

        var (totalCost, segments) = CalculatePrice(request.StartTime, request.EndTime, rules);
        return new PricePreviewResponse(totalCost, segments);
    }

    public async Task<BookingDto> CreateBookingAsync(
        CreateBookingRequest request, CancellationToken ct = default)
    {
        if (currentUser.ActorType != ActorType.Customer || currentUser.ActorId is null)
            throw new UnauthorizedException("Only authenticated customers can create bookings.");

        var customer = await customerRepository.GetByAccountIdAsync(currentUser.ActorId.Value, ct)
            ?? throw new UnauthorizedException("Customer profile not found.");

        return await CreateBookingCoreAsync(
            customer, request.CourtId, request.Date,
            request.StartTime, request.EndTime,
            request.ConfirmedTotalCost,
            ActorType.Customer, customer.CustomerId, null, ct);
    }

    public async Task<BookingDto> QuickBookingAsync(
        QuickBookingRequest request, CancellationToken ct = default)
    {
        ValidateContactInfo(request.FullName, request.Phone, request.Email);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var code = await verificationCodeRepository.GetActiveAsync(
            normalizedEmail, CodeType.QuickBooking, ct);

        if (code is null || code.IsUsed || code.ExpiresAt <= clock.GetUtcNow().UtcDateTime)
            throw new ValidationException("Verification code is invalid or expired.",
                new Dictionary<string, string[]> { ["VerificationCode"] = ["Invalid or expired code."] });

        var codeHash = HashCode(request.VerificationCode);
        if (code.CodeHash != codeHash)
        {
            code.Attempt++;
            if (code.Attempt >= 5)
                code.ExpiresAt = clock.GetUtcNow().UtcDateTime;
            await unitOfWork.SaveChangesAsync(ct);
            throw new ValidationException("Verification code is incorrect.",
                new Dictionary<string, string[]> { ["VerificationCode"] = ["Incorrect code."] });
        }

        var customer = await customerRepository.GetByEmailAsync(normalizedEmail, ct);
        if (customer is null)
        {
            customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                Phone = NormalizePhone(request.Phone),
                Email = normalizedEmail,
                CreatedAt = clock.GetUtcNow().UtcDateTime,
                UpdatedAt = clock.GetUtcNow().UtcDateTime
            };
            await unitOfWork.AddAsync(customer, ct);
        }

        var booking = await CreateBookingCoreAsync(
            customer, request.CourtId, request.Date,
            request.StartTime, request.EndTime,
            request.ConfirmedTotalCost,
            ActorType.Customer, customer.CustomerId, null, ct,
            consumeCode: code);

        return booking;
    }

    public async Task<BookingDto> CancelBookingAsync(
        Guid bookingId, CancelBookingRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ValidationException("Reason is required.",
                new Dictionary<string, string[]> { ["Reason"] = ["Reason is required."] });

        var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId, ct)
            ?? throw new NotFoundException("Booking not found.");

        if (currentUser.ActorType == ActorType.Customer)
        {
            var customer = currentUser.ActorId is not null
                ? await customerRepository.GetByAccountIdAsync(currentUser.ActorId.Value, ct)
                : null;
            if (customer is null || booking.CustomerId != customer.CustomerId)
                throw new NotFoundException("Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
            return BookingDto.FromEntity(booking);

        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed))
            throw new ConflictException("Only pending or confirmed bookings can be cancelled.");

        if (currentUser.ActorType == ActorType.Customer)
        {
            var invoice = booking.Invoices.FirstOrDefault(i =>
                i.Status is InvoiceStatus.Paid or InvoiceStatus.Unpaid);
            if (invoice?.Status == InvoiceStatus.Paid)
                throw new ConflictException(
                    "Paid bookings must be cancelled through an employee.");
        }

        var nowUtc = clock.GetUtcNow().UtcDateTime;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(
            booking.Date.ToDateTime(booking.StartTime), VietnamTz);
        if (nowUtc > startUtc.AddHours(-1))
            throw new ConflictException(
                "Booking can only be cancelled at least 1 hour before start time.");

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = nowUtc;

        var unpaidInvoice = booking.Invoices.FirstOrDefault(i => i.Status == InvoiceStatus.Unpaid);
        if (unpaidInvoice is not null)
        {
            unpaidInvoice.Status = InvoiceStatus.Cancelled;
            unpaidInvoice.CancelledAt = nowUtc;
            unpaidInvoice.CancellationActorType = currentUser.ActorType;
            unpaidInvoice.CancellationReason = request.Reason;
            if (currentUser.ActorType == ActorType.Customer)
            {
                var customer = await customerRepository.GetByAccountIdAsync(currentUser.ActorId!.Value, ct);
                unpaidInvoice.CancelledByCustomer = customer?.CustomerId;
            }
        }

        var history = new BookingStatusHistory
        {
            Id = Guid.NewGuid(),
            BookingId = booking.BookingId,
            OldStatus = oldStatus,
            NewStatus = BookingStatus.Cancelled,
            ActorType = currentUser.ActorType,
            ChangedAt = nowUtc,
            Reason = request.Reason
        };

        switch (currentUser.ActorType)
        {
            case ActorType.Customer:
                {
                    var customer = await customerRepository.GetByAccountIdAsync(currentUser.ActorId!.Value, ct);
                    history.CustomerId = customer?.CustomerId;
                    break;
                }
            case ActorType.Employee:
                history.EmployeeId = currentUser.ActorId;
                break;
        }

        await unitOfWork.AddAsync(history, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return BookingDto.FromEntity(booking);
    }

    // US-7: Get booking history for authenticated customer
    public async Task<PagedResult<BookingDto>> GetMyBookingsAsync(
        PageRequest page, CancellationToken ct = default)
    {
        if (currentUser.ActorType != ActorType.Customer || currentUser.ActorId is null)
            throw new UnauthorizedException("Only authenticated customers can view booking history.");

        var customer = await customerRepository.GetByAccountIdAsync(currentUser.ActorId.Value, ct)
            ?? throw new UnauthorizedException("Customer profile not found.");

        var paged = await bookingRepository.GetByCustomerAsync(customer.CustomerId, page, ct);

        var dtos = paged.Items.Select(BookingDto.FromEntity).ToList();
        return new PagedResult<BookingDto>
        {
            Items = dtos,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    private async Task<BookingDto> CreateBookingCoreAsync(
        Customer customer,
        int courtId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        decimal? confirmedTotalCost,
        ActorType actorType,
        Guid? actorCustomerId,
        Guid? actorEmployeeId,
        CancellationToken ct,
        VerificationCode? consumeCode = null)
    {
        ValidateTimeSlot(startTime, endTime);

        var nowUtc = clock.GetUtcNow().UtcDateTime;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(startTime), VietnamTz);

        if (startUtc <= nowUtc)
            throw new ConflictException("Booking start time must be in the future.");

        var isoDow = GetIsoDayOfWeek(date);
        var schedule = await bookingRepository.GetScheduleAsync(courtId, isoDow, ct);
        ValidateSchedule(schedule, startTime, endTime);

        var rules = await bookingRepository.GetPricingRulesAsync(courtId, isoDow, ct);
        var (totalCost, segments) = CalculatePrice(startTime, endTime, rules);

        if (confirmedTotalCost.HasValue && confirmedTotalCost.Value != totalCost)
            throw new ConflictException(
                $"Price has changed. New total cost is {totalCost} VND. Please confirm the new price.");

        var bookingCode = await GenerateUniqueCodeAsync(ct);

        var booking = new Booking
        {
            BookingId = Guid.NewGuid(),
            BookingCode = bookingCode,
            CustomerId = customer.CustomerId,
            CourtId = courtId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            Status = BookingStatus.Pending,
            TotalCost = totalCost,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };

        await unitOfWork.AddAsync(booking, ct);

        foreach (var seg in segments)
        {
            await unitOfWork.AddAsync(new BookingPriceSegment
            {
                BookingId = booking.BookingId,
                SegmentNo = seg.SegmentNo,
                PriceStartTime = seg.PriceStartTime,
                SegmentStartTime = seg.SegmentStartTime,
                SegmentEndTime = seg.SegmentEndTime,
                PricePerHour = seg.PricePerHour
            }, ct);
        }

        var history = new BookingStatusHistory
        {
            Id = Guid.NewGuid(),
            BookingId = booking.BookingId,
            OldStatus = null,
            NewStatus = BookingStatus.Pending,
            ActorType = actorType,
            CustomerId = actorCustomerId,
            EmployeeId = actorEmployeeId,
            ChangedAt = nowUtc,
            Reason = "Booking created"
        };
        await unitOfWork.AddAsync(history, ct);

        if (consumeCode is not null)
        {
            consumeCode.IsUsed = true;
            consumeCode.ExpiresAt = nowUtc;
        }

        await unitOfWork.SaveChangesAsync(ct);

        return BookingDto.FromEntity(booking);
    }

    private static (decimal TotalCost, IReadOnlyList<BookingPriceSegmentDto> Segments)
        CalculatePrice(TimeOnly startTime, TimeOnly endTime, List<PricingRule> rules)
    {
        if (rules.Count == 0)
            throw new ConflictException("No pricing rules configured for this court and day.");

        var sorted = rules.OrderBy(r => r.StartTime).ToList();
        var segments = new List<BookingPriceSegmentDto>();
        var segNo = 1;
        decimal totalCostRaw = 0;

        for (var i = 0; i < sorted.Count; i++)
        {
            var rule = sorted[i];
            var ruleEnd = i + 1 < sorted.Count ? sorted[i + 1].StartTime : TimeOnly.MaxValue;

            var segStart = startTime > rule.StartTime ? startTime : rule.StartTime;
            var segEnd = endTime < ruleEnd ? endTime : ruleEnd;

            if (segStart >= segEnd) continue;

            var minutes = (segEnd - segStart).TotalMinutes;
            totalCostRaw += (decimal)minutes * rule.PricePerHour / 60m;

            segments.Add(new BookingPriceSegmentDto(
                segNo++,
                rule.StartTime,
                segStart,
                segEnd,
                rule.PricePerHour));
        }

        if (segments.Count == 0)
            throw new ConflictException("Booking time does not fall within any pricing rule.");

        var totalCost = Math.Round(totalCostRaw, 0, MidpointRounding.AwayFromZero);
        return (totalCost, segments);
    }

    private static void ValidateTimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        var errors = new Dictionary<string, string[]>();

        if (startTime.Minute != 0 && startTime.Minute != 30)
            errors["StartTime"] = ["Start time must be at :00 or :30."];
        if (endTime.Minute != 0 && endTime.Minute != 30)
            errors["EndTime"] = ["End time must be at :00 or :30."];
        if (startTime >= endTime)
            errors["EndTime"] = ["End time must be after start time."];
        if ((endTime - startTime).TotalMinutes < 30)
            errors["EndTime"] = ["Minimum booking duration is 30 minutes."];

        if (errors.Count > 0)
            throw new ValidationException("Invalid time slot.", errors);
    }

    private static void ValidateSchedule(CourtSchedule? schedule, TimeOnly startTime, TimeOnly endTime)
    {
        if (schedule is null || !schedule.IsAvailable)
            throw new ConflictException("Court is not available on this day.");
        if (startTime < schedule.OpenTime || endTime > schedule.CloseTime)
            throw new ConflictException(
                $"Booking must be within court hours ({schedule.OpenTime}–{schedule.CloseTime}).");
    }

    private static void ValidateContactInfo(string fullName, string phone, string email)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(fullName))
            errors["FullName"] = ["Full name is required."];
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            errors["Email"] = ["A valid email is required."];
        if (string.IsNullOrWhiteSpace(phone))
            errors["Phone"] = ["Phone number is required."];
        else
        {
            var digits = phone.Replace(" ", "").Replace("-", "");
            var digitOnly = digits.StartsWith('+') ? digits[1..] : digits;
            if (!digitOnly.All(char.IsDigit) || digitOnly.Length < 8 || digitOnly.Length > 12)
                errors["Phone"] = ["Phone must be 8–12 digits (leading + allowed)."];
        }

        if (errors.Count > 0)
            throw new ValidationException("Invalid contact information.", errors);
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var suffix = RandomNumberGenerator.GetString(chars, 6);
            var code = $"DS-{suffix}";
            if (!await bookingRepository.CodeExistsAsync(code, ct))
                return code;
        }
        throw new InvalidOperationException("Failed to generate a unique booking code.");
    }

    private static int GetIsoDayOfWeek(DateOnly date) =>
        date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;

    private static string NormalizePhone(string phone) =>
        phone.Replace(" ", "").Replace("-", "");

    private static string HashCode(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim()));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

}
