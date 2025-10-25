using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO;

public class CommitDiffDto
{
    [JsonPropertyName("diff")]
    public string Diff { get; set; } = string.Empty;
}