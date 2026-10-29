using System.Security.Cryptography;
using System.Text;

namespace Clone.WebApi.Services;

/// <summary>
/// Validates the X-Hub-Signature-256 header (HMAC-SHA256 of the raw body using the App Secret).
/// Reference: https://developers.facebook.com/documentation/business-messaging/whatsapp/webhooks/create-webhook-endpoint
/// </summary>
public static class WhatsAppSignatureValidator
{
    private const string SHA256_PREFIX = "sha256=";

    public static bool IsValid(byte[] body, string? signatureHeader, string? appSecret)
    {
        if (body.Length == 0 || string.IsNullOrEmpty(appSecret) || string.IsNullOrWhiteSpace(signatureHeader))
        {
            return false;
        }

        if (!signatureHeader.StartsWith(SHA256_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var received = signatureHeader[SHA256_PREFIX.Length..].Trim();
        if (received.Length != 64) // hex of 32 bytes
        {
            return false;
        }

        var expected = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(appSecret), body));

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expected),
            Encoding.ASCII.GetBytes(received.ToUpperInvariant()));
    }
}
