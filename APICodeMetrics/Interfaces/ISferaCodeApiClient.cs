using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Interfaces;

public interface ISferaCodeApiClient
{
    Task<SferaCodeResponseWrapper<ProjectDto[]>> GetProjectsAsync(int start = 0, int limit = 25, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<RepositoryDto[]>> GetRepositoriesAsync(string projectKey, int start = 0, int limit = 25, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<BranchDto[]>> GetBranchesAsync(string projectKey, string repoName, int start = 0, int limit = 25, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<CommitDto[]>> GetCommitsAsync(string projectKey, string repoName, string? branchName = null, int start = 0, int limit = 25, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<CommitDetailsDto>> GetCommitAsync(string projectKey, string repoName, string sha1, CancellationToken cancellationToken = default);
    Task<SferaCodeResponseWrapper<CommitDiffDto>> GetCommitDiffAsync(string projectKey, string repoName, string sha1, CancellationToken cancellationToken = default);
}