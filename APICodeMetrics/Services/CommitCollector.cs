using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class CommitCollector : IDataCollector<CommitDto[]>
{
    private readonly ISferaCodeApiClient _apiClient;
    private readonly ILogger<CommitCollector> _logger;

    public CommitCollector(ISferaCodeApiClient apiClient, ILogger<CommitCollector> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<CommitDto[]> CollectAsync(object context, CancellationToken cancellationToken = default)
    {
        var result = context as (ProjectDto Project, RepositoryDto Repository, BranchDto Branch)?;
        if (result is not { } branchContext)
        {
            throw new ArgumentException("Context must be a (ProjectDto Project, RepositoryDto Repository, BranchDto Branch) tuple.", nameof(context));
        }

        var (project, repo, branch) = branchContext;

        _logger.LogInformation("Collecting commits for branch: {BranchName} in repository: {RepoName}", branch.Name, repo.Name);
        try
        {
            var response = await _apiClient.GetCommitsAsync(project.Name, repo.Name, branch.Name, 0, int.MaxValue, cancellationToken);
            _logger.LogInformation("Successfully collected {CommitCount} commits for branch {BranchName}.", response.Data?.Length ?? 0, branch.Name);
            return response.Data ?? Array.Empty<CommitDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during collection of commits for branch {BranchName} in repository {RepoName}.", branch.Name, repo.Name);
            throw;
        }
    }
}