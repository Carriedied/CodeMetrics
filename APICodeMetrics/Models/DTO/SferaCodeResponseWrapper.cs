using System.Text.Json.Serialization;

namespace APICodeMetrics.Models.DTO;

public class SferaCodeResponseWrapper<T>
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}