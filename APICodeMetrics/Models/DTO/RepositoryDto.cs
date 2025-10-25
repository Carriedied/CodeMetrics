using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO;

public class RepositoryDto
{
    [JsonPropertyName("clone_links")]
    public CloneLinks CloneLinks { get; set; } = new();

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fork_slug")]
    public ForkSlug ForkSlug { get; set; } = new();

    [JsonPropertyName("is_fork")]
    public bool IsFork { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("owner_name")]
    public string OwnerName { get; set; } = string.Empty;

    [JsonPropertyName("topics")]
    public string[] Topics { get; set; } = Array.Empty<string>();

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;
}