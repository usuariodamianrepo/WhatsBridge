using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Metadados da conta de negocio que recebeu a mensagem.
/// </summary>
public class WhatsAppWebhookMetadataDto
{
    /// <summary>BUSINESS_DISPLAY_PHONE_NUMBER (ex.: "15550783881").</summary>
    [JsonPropertyName("display_phone_number")]
    public string DisplayPhoneNumber { get; set; } = string.Empty;

    /// <summary>BUSINESS_PHONE_NUMBER_ID (ex.: "106540352242922").</summary>
    [JsonPropertyName("phone_number_id")]
    public string PhoneNumberId { get; set; } = string.Empty;
}
