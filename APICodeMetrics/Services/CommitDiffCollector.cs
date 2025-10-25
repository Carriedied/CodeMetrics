using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class CommitDiffCollector : IDataCollector<CommitDiffDto>
{
    private readonly ISferaCodeApiClient _apiClient;
    private readonly ILogger<CommitDiffCollector> _logger;

    public CommitDiffCollector(ISferaCodeApiClient apiClient, ILogger<CommitDiffCollector> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<CommitDiffDto> CollectAsync(object context, CancellationToken cancellationToken = default)
    {
        var result = context as (ProjectDto Project, RepositoryDto Repository, BranchDto Branch, CommitDto Commit)?;
        if (result is not { } commitContext)
        {
            throw new ArgumentException("Context must be a (ProjectDto Project, RepositoryDto Repository, BranchDto Branch, CommitDto Commit) tuple.", nameof(context));
        }

        var (project, repo, branch, commit) = commitContext;

        // --- ИЗМЕНЕНО: Если Sha1 пустой, возвращаем пустой DTO без вызова API ---
        if (string.IsNullOrEmpty(commit.Sha1))
        {
            _logger.LogWarning("Commit has empty or null SHA1 in branch {BranchName} of repository {RepoName}. Returning empty diff.", branch.Name, repo.Name);
            // Возвращаем "пустой" объект DTO
            return new CommitDiffDto(); // Возвращаем пустой DTO
        }
        // ---

        _logger.LogInformation("Collecting diff for commit: {CommitSha} in branch: {BranchName}", commit.Sha1, branch.Name);
        try
        {
            var response = await _apiClient.GetCommitDiffAsync(project.Name, repo.Name, commit.Sha1, cancellationToken);
            _logger.LogInformation("Successfully retrieved diff for commit {CommitSha}.", commit.Sha1);
            return response.Data ?? new CommitDiffDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during collection of diff for commit {CommitSha} in branch {BranchName}.", commit.Sha1, branch.Name);
            // Возвращаем пустой объект при ошибке
            return new CommitDiffDto();
        }
    }
}