using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO.Branch;

public class Author
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}