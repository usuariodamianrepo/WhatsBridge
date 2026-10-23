using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Alteracao notificada pelo webhook. Para pedidos, o campo e "messages".
/// </summary>
public class WhatsAppWebhookChangeDto
{
    [JsonPropertyName("value")]
    public WhatsAppWebhookValueDto Value { get; set; } = new();

    /// <summary>Campo que disparou a notificacao (ex.: "messages").</summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
}
