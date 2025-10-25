using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO;

public class CloneLinks
{
    [JsonPropertyName("https")]
    public string Https { get; set; } = string.Empty;

    [JsonPropertyName("ssh")]
    public string Ssh { get; set; } = string.Empty;
}