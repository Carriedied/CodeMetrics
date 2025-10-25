using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO;

public class ForkSlug
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;
}