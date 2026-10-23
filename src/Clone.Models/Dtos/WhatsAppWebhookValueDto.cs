using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Conteudo da notificacao: metadados do negocio, contatos e mensagens recebidas.
/// </summary>
public class WhatsAppWebhookValueDto
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public WhatsAppWebhookMetadataDto Metadata { get; set; } = new();

    [JsonPropertyName("contacts")]
    public List<WhatsAppContactDto> Contacts { get; set; } = [];

    [JsonPropertyName("messages")]
    public List<WhatsAppMessageDto> Messages { get; set; } = [];
}
