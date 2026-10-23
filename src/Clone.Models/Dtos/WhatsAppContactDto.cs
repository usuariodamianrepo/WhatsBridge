using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Usuario de WhatsApp que enviou a mensagem.
/// </summary>
public class WhatsAppContactDto
{
    [JsonPropertyName("profile")]
    public WhatsAppContactProfileDto Profile { get; set; } = new();

    /// <summary>WHATSAPP_USER_ID. Pode nao coincidir com o numero de telefone.</summary>
    [JsonPropertyName("wa_id")]
    public string WaId { get; set; } = string.Empty;

    /// <summary>
    /// IDENTITY_KEY_HASH. So e incluido se a verificacao de mudanca de identidade estiver habilitada.
    /// </summary>
    [JsonPropertyName("identity_key_hash")]
    public string? IdentityKeyHash { get; set; }
}
