using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Conteudo do pedido realizado pelo usuario a partir do catalogo.
/// </summary>
public class WhatsAppOrderMessageDto
{
    /// <summary>PRODUCT_CATALOG_ID (ID do catalogo de produtos).</summary>
    [JsonPropertyName("catalog_id")]
    public string CatalogId { get; set; } = string.Empty;

    /// <summary>ORDER_TEXT (texto que acompanha o pedido, ex.: "Love these!").</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("product_items")]
    public List<WhatsAppOrderProductItemDto> ProductItems { get; set; } = [];
}
