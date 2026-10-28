using System.Security.Cryptography;
using System.Text;

namespace Clone.WebApi.Services;

/// <summary>
/// Valida o header X-Hub-Signature-256 (HMAC-SHA256 do corpo bruto com o App Secret).
/// Referencia: https://developers.facebook.com/documentation/business-messaging/whatsapp/webhooks/create-webhook-endpoint
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
        if (received.Length != 64) // hex de 32 bytes
        {
            return false;
        }

        var expected = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(appSecret), body));

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expected),
            Encoding.ASCII.GetBytes(received.ToUpperInvariant()));
    }
}
