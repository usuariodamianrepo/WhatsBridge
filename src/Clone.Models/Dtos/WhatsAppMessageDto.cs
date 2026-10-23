using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Mensagem recebida. Neste webhook corresponde a um pedido (type = "order").
/// </summary>
public class WhatsAppMessageDto
{
    /// <summary>WHATSAPP_USER_PHONE_NUMBER (numero que enviou a mensagem).</summary>
    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    /// <summary>WHATSAPP_MESSAGE_ID (ex.: "wamid.HBgLMTY1MDM4Nzk0MzkVAgAS...").</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>WEBHOOK_TRIGGER_TIMESTAMP (Unix timestamp em segundos, como texto).</summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    /// <summary>Tipo da mensagem (ex.: "order").</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Detalhe do pedido. So esta presente quando <see cref="Type"/> e "order".</summary>
    [JsonPropertyName("order")]
    public WhatsAppOrderMessageDto? Order { get; set; }
}
