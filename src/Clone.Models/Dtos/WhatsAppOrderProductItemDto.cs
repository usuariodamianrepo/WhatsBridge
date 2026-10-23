using System.Text.Json.Serialization;

namespace Clone.Models.Dtos;

/// <summary>
/// Produto incluido no pedido.
/// </summary>
public class WhatsAppOrderProductItemDto
{
    /// <summary>PRODUCT_ID (ID do produto no catalogo).</summary>
    [JsonPropertyName("product_retailer_id")]
    public string ProductRetailerId { get; set; } = string.Empty;

    /// <summary>PRODUCT_QUANTITY.</summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>PRODUCT_PRICE (preco unitario; pode ter decimais, ex.: 7.99).</summary>
    [JsonPropertyName("item_price")]
    public decimal ItemPrice { get; set; }

    /// <summary>CURRENCY_CODE (ex.: "USD").</summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}
