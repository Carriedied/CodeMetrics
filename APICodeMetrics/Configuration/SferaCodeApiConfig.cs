namespace APICodeMetrics.Configuration;

public class SferaCodeApiConfig
{
    public const string SectionName = "SferaCodeApi";
    public string BaseUrl { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}