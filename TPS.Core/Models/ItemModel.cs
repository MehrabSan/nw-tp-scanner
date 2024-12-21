using System.Text.Json.Serialization;

namespace TPS.Core.Models;

public class ItemModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("tradingCategory")]
    public string TradingCategory { get; set; }
    public int MaxStackSize { get; set; }
}