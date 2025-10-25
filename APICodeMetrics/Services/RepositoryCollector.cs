using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class RepositoryCollector(ISferaCodeApiClient apiClient, ILogger<RepositoryCollector> logger)
    : IDataCollector<RepositoryDto[]>
{
    public async Task<RepositoryDto[]> CollectAsync(object context, CancellationToken cancellationToken = default)
    {
        if (context is not ProjectDto project)
        {
            throw new ArgumentException("Context must be a ProjectDto.", nameof(context));
        }

        logger.LogInformation("Collecting repositories for project: {ProjectName}", project.Name);
        try
        {
            var response = await apiClient.GetRepositoriesAsync(project.Name, 0, int.MaxValue, cancellationToken);
            logger.LogInformation("Successfully collected {RepoCount} repositories for project {ProjectName}.", response.Data?.Length ?? 0, project.Name);
            return response.Data ?? Array.Empty<RepositoryDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during collection of repositories for project {ProjectName}.", project.Name);
            throw;
        }
    }
}