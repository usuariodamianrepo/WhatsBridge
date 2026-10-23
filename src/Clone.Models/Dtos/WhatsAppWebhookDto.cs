using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Payload raiz do webhook de mensagens (messaging account) do WhatsApp Business.
/// Referencia: https://developers.facebook.com/documentation/business-messaging/whatsapp/webhooks/reference/messages/order
/// </summary>
public class WhatsAppWebhookDto
{
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("insert_date")]
    public DateTime InsertDate { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("entry")]
    public List<WhatsAppWebhookEntryDto> Entry { get; set; } = [];
}
