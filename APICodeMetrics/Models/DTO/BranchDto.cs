using System.Text.Json.Serialization;
using APICodeMetrics.Models.DTO.Branch;

namespace APICodeMetrics.Models.DTO;

public class BranchDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("commit")]
    public CommitPerson Commit { get; set; } = new();

    [JsonPropertyName("protected")]
    public bool Protected { get; set; }

    [JsonPropertyName("target_branch")]
    public string TargetBranch { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;
}