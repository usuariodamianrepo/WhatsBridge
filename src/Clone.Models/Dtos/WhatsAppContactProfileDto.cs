using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Perfil do usuario de WhatsApp tal como aparece no cliente.
/// </summary>
public class WhatsAppContactProfileDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
