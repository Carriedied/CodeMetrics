using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using APICodeMetrics.Configuration;
using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using Microsoft.Extensions.Options;

namespace APICodeMetrics.Services;

public class SferaCodeApiClient : ISferaCodeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly SferaCodeApiConfig _config;
    private readonly ILogger<SferaCodeApiClient> _logger;

    public SferaCodeApiClient(HttpClient httpClient, IOptions<SferaCodeApiConfig> config, ILogger<SferaCodeApiClient> logger)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        var credentials = $"{_config.Login}:{_config.Password}";
        var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
    }

    public async Task<SferaCodeResponseWrapper<ProjectDto[]>> GetProjectsAsync(int start = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        const string url = $"projects"; // Пока без пагинации в URL, так как в примере она не используется
        _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
        _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogDebug("Response Body from /projects: {JsonResponse}", json);

        var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<ProjectDto[]>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
       return wrapper ?? new SferaCodeResponseWrapper<ProjectDto[]> { Data = Array.Empty<ProjectDto>() };
    }

    public async Task<SferaCodeResponseWrapper<RepositoryDto[]>> GetRepositoriesAsync(string projectKey, int start = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        var url = $"projects/{projectKey}/repos"; // Пока без пагинации в URL
        _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
        _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogDebug("Response Body from /projects/{ProjectKey}/repos: {JsonResponse}", projectKey, json);

        var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<RepositoryDto[]>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return wrapper ?? new SferaCodeResponseWrapper<RepositoryDto[]> { Data = Array.Empty<RepositoryDto>() };
    }
    
    public async Task<SferaCodeResponseWrapper<BranchDto[]>> GetBranchesAsync(string projectKey, string repoName, int start = 0, int limit = 25, CancellationToken cancellationToken = default)
        {
            var url = $"projects/{projectKey}/repos/{repoName}/branches"; // Пока без пагинации в URL
            _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
            _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Response Body from /projects/{ProjectKey}/repos/{RepoName}/branches: {JsonResponse}", projectKey, repoName, json);

            var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<BranchDto[]>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return wrapper ?? new SferaCodeResponseWrapper<BranchDto[]> { Data = Array.Empty<BranchDto>() };
        }

        public async Task<SferaCodeResponseWrapper<CommitDto[]>> GetCommitsAsync(string projectKey, string repoName, string? branchName = null, int start = 0, int limit = 25, CancellationToken cancellationToken = default)
        {
            var url = $"projects/{projectKey}/repos/{repoName}/commits";
            if (!string.IsNullOrEmpty(branchName))
            {
                url += $"?branchName={branchName}"; // Проверьте документацию API на этот счет
            }
            // Или, если API использует параметры start/limit в URL для commits
            // url += $"?start={start}&limit={limit}";

            _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
            _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Response Body from /projects/{ProjectKey}/repos/{RepoName}/commits: {JsonResponse}", projectKey, repoName, json);

            var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<CommitDto[]>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return wrapper ?? new SferaCodeResponseWrapper<CommitDto[]> { Data = Array.Empty<CommitDto>() };
        }

        public async Task<SferaCodeResponseWrapper<CommitDetailsDto>> GetCommitAsync(string projectKey, string repoName, string sha1, CancellationToken cancellationToken = default)
        {
            var url = $"projects/{projectKey}/repos/{repoName}/commits/{sha1}";
            _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
            _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Response Body from /projects/{ProjectKey}/repos/{RepoName}/commits/{Sha1}: {JsonResponse}", projectKey, repoName, sha1, json);

            var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<CommitDetailsDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return wrapper ?? new SferaCodeResponseWrapper<CommitDetailsDto> { Data = new CommitDetailsDto() };
        }

        public async Task<SferaCodeResponseWrapper<CommitDiffDto>> GetCommitDiffAsync(string projectKey, string repoName, string sha1, CancellationToken cancellationToken = default)
        {
            var url = $"projects/{projectKey}/repos/{repoName}/commits/{sha1}/diff";
            _logger.LogDebug("Making GET request to: {BaseUrl}{Url}", _httpClient.BaseAddress, url);
            _logger.LogDebug("Authorization Header: {AuthHeader}", _httpClient.DefaultRequestHeaders.Authorization);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Response Body from /projects/{ProjectKey}/repos/{RepoName}/commits/{Sha1}/diff: {JsonResponse}", projectKey, repoName, sha1, json);

            var wrapper = JsonSerializer.Deserialize<SferaCodeResponseWrapper<CommitDiffDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return wrapper ?? new SferaCodeResponseWrapper<CommitDiffDto> { Data = new CommitDiffDto() };
        }
}