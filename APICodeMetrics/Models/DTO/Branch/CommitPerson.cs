using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO.Branch;

public class CommitPerson
{
    [JsonPropertyName("author")]
    public Author Author { get; set; } = new();

    [JsonPropertyName("committer")]
    public Author Committer { get; set; } = new();

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("sha1")]
    public string Sha1 { get; set; } = string.Empty;
}