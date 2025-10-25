using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Interfaces;

public interface ISferaCodeApiClient
{
    Task<SferaCodeResponseWrapper<ProjectDto[]>> GetProjectsAsync(int start = 0, int limit = 25, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<RepositoryDto[]>> GetRepositoriesAsync(string projectKey, int start = 0, int limit = 25, CancellationToken cancellationToken = default);
}