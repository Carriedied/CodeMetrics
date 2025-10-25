using System.Text.Json.Serialization;
using APICodeMetrics.Models.DTO.Branch;

namespace APICodeMetrics.Models.DTO;

public class CommitDto
{
    [JsonPropertyName("author")]
    public Author Author { get; set; } = new();

    [JsonPropertyName("committer")]
    public Author Committer { get; set; } = new();

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("sha1")]
    public string Sha1 { get; set; } = string.Empty;

    [JsonPropertyName("parent_sha1")]
    public string ParentSha1 { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;
}