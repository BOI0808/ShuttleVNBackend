using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Users;

namespace ShuttleVNBackend.Application.DTOs.Bookings;

public record QuickLookupResultDto(
    QuickLookupMatchType MatchType,
    IReadOnlyList<CustomerListItemDto> Customers,
    PagedResult<BookingListItemDto> Bookings);