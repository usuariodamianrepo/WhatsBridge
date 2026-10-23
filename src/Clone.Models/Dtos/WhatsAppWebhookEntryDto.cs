using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Entrada do webhook. Contem o ID da conta de negocio do WhatsApp e as alteracoes recebidas.
/// </summary>
public class WhatsAppWebhookEntryDto
{
    /// <summary>WHATSAPP_BUSINESS_ACCOUNT_ID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("changes")]
    public List<WhatsAppWebhookChangeDto> Changes { get; set; } = [];
}
