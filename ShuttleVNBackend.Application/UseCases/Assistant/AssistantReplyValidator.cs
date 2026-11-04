using System.Text.RegularExpressions;

namespace ShuttleVNBackend.Application.UseCases.Assistant;

/// <summary>
/// Guardrail for model output: the assistant is advisory only, so any claim of a
/// completed business action or an invented booking code is replaced by a safe reply.
/// </summary>
public static partial class AssistantReplyValidator
{
    public const string FallbackReply =
        "Mình chỉ tư vấn, không thực hiện đặt, hủy hay thanh toán. " +
        "Bạn dùng màn Đặt sân trên website hoặc liên hệ nhân viên để được hỗ trợ.";

    private static readonly string[] ForbiddenClaims =
    [
        "đã đặt sân thành công", "đã giữ chỗ", "đã giữ sân", "đã hủy đơn",
        "đã hủy sân", "đã đổi lịch thành công", "đã thanh toán", "đã thu tiền",
        "đã hoàn tiền", "đã mở khóa tài khoản", "đơn của bạn đã được xác nhận"
    ];

    public static string Sanitize(string? content, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return FallbackReply;
        }

        var text = content.Trim();
        var lower = text.ToLowerInvariant();

        if (BookingCodePattern().IsMatch(text))
        {
            return FallbackReply;
        }

        foreach (var claim in ForbiddenClaims)
        {
            if (lower.Contains(claim, StringComparison.Ordinal))
            {
                return FallbackReply;
            }
        }

        return text.Length <= maxChars ? text : Truncate(text, maxChars);
    }

    private static string Truncate(string text, int maxChars)
    {
        var cut = text[..maxChars];
        var lastStop = cut.LastIndexOfAny(['.', '!', '?']);

        return lastStop > 0 ? cut[..(lastStop + 1)] : cut + "…";
    }

    [GeneratedRegex(@"\bDS-[A-Za-z0-9]{5,}\b")]
    private static partial Regex BookingCodePattern();
}
