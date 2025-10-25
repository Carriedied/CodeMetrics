using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class ProjectCollector(ISferaCodeApiClient apiClient, ILogger<ProjectCollector> logger)
    : IDataCollector<ProjectDto[]>
{
    public async Task<ProjectDto[]> CollectAsync(object context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Collecting projects.");
        try
        {
            var response = await apiClient.GetProjectsAsync(0, int.MaxValue, cancellationToken);
            logger.LogInformation("Successfully collected {ProjectCount} projects.", response.Data?.Length ?? 0);
            return response.Data ?? Array.Empty<ProjectDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during collection of projects.");
            throw;
        }
    }
}