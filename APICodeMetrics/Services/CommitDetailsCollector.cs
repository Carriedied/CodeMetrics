using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class CommitDetailsCollector : IDataCollector<CommitDetailsDto>
{
    private readonly ISferaCodeApiClient _apiClient;
    private readonly ILogger<CommitDetailsCollector> _logger;

    public CommitDetailsCollector(ISferaCodeApiClient apiClient, ILogger<CommitDetailsCollector> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<CommitDetailsDto> CollectAsync(object context, CancellationToken cancellationToken = default)
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
                 _logger.LogWarning("Commit has empty or null SHA1 in branch {BranchName} of repository {RepoName}. Returning empty details.", branch.Name, repo.Name);
                 // Возвращаем "пустой" объект DTO
                 return new CommitDetailsDto(); // Возвращаем пустой DTO
             }
             // ---

             _logger.LogInformation("Collecting details for commit: {CommitSha} in branch: {BranchName}", commit.Sha1, branch.Name);
             try
             {
                 var response = await _apiClient.GetCommitAsync(project.Name, repo.Name, commit.Sha1, cancellationToken);
                 _logger.LogInformation("Successfully retrieved details for commit {CommitSha}.", commit.Sha1);
                 return response.Data ?? new CommitDetailsDto();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "An error occurred during collection of details for commit {CommitSha} in branch {BranchName}.", commit.Sha1, branch.Name);
                 // Важно: возвращаем пустой объект даже при ошибке API, если нужно сохранить "неполные" данные
                 // Или можно решить выбрасывать исключение, если ошибка критична.
                 // Для сохранения "неполных" данных - возвращаем заглушку.
                 return new CommitDetailsDto();
             }
        }
}