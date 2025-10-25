using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class RepositoryCollectorService(ISferaCodeApiClient apiClient, ILogger<RepositoryCollectorService> logger)
    : IRepositoryCollectorService
{
    public async Task<SferaCodeResponseWrapper<RepositoryDto[]>> CollectAllRepositoriesForProjectAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting collection of repositories for project: {ProjectKey}", projectKey);
        try
        {
            var response = await apiClient.GetRepositoriesAsync(projectKey, 0, int.MaxValue, cancellationToken); // Получаем все репозитории за один раз

            logger.LogInformation("Successfully collected {RepoCount} repositories for project {ProjectKey}.", response.Data?.Length ?? 0, projectKey);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during collection of repositories for project {ProjectKey}.", projectKey);
            throw; // Пробрасываем исключение дальше
        }
    }
}