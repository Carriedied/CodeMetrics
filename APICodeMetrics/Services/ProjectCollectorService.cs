using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class ProjectCollectorService(ISferaCodeApiClient apiClient, ILogger<ProjectCollectorService> logger) 
    : IProjectCollectorService
{
    public async Task<SferaCodeResponseWrapper<ProjectDto[]>> CollectAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting collection of all projects.");
        try
        {
            var response = await apiClient.GetProjectsAsync(0, int.MaxValue, cancellationToken); // Получаем все проекты за один раз

            logger.LogInformation("Successfully collected {ProjectCount} projects.", response.Data?.Length ?? 0);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during collection of projects.");
            throw; // Пробрасываем исключение дальше, чтобы контроллер мог его обработать
        }
    }
}