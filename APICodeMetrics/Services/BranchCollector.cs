using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using APICodeMetrics.Models;

namespace APICodeMetrics.Services;

public class BranchCollector : IDataCollector<BranchDto[]>
{
    private readonly ISferaCodeApiClient _apiClient;
    private readonly ILogger<BranchCollector> _logger;

    public BranchCollector(ISferaCodeApiClient apiClient, ILogger<BranchCollector> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<BranchDto[]> CollectAsync(object context, CancellationToken cancellationToken = default)
    {
        // 1. Попробовать привести object к кортежу (ProjectDto, RepositoryDto)
        // Используем 'as' для безопасного приведения. Если не получится, result будет null.
        var result = context as (ProjectDto Project, RepositoryDto Repository)?;

        // 2. Проверить, удалось ли привести (result != null) и не равно ли это null-кортежу (если context был null)
        if (result is not { } repoContext) // Проверка на null (как кортеж, так и его значение)
        {
            // Если приведение не удалось или context был null, бросаем исключение
            throw new ArgumentException("Context must be a (ProjectDto Project, RepositoryDto Repository) tuple.", nameof(context));
        }

        // 3. Теперь repoContext - это кортеж нужного типа, можно деструктурировать
        var (project, repo) = repoContext;

        _logger.LogInformation("Collecting branches for repository: {RepoName} in project: {ProjectName}", repo.Name, project.Name);
        try
        {
            var response = await _apiClient.GetBranchesAsync(project.Name, repo.Name, 0, int.MaxValue, cancellationToken);
            _logger.LogInformation("Successfully collected {BranchCount} branches for repository {RepoName}.", response.Data?.Length ?? 0, repo.Name);
            return response.Data ?? Array.Empty<BranchDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during collection of branches for repository {RepoName} in project {ProjectName}.", repo.Name, project.Name);
            throw;
        }
    }
}