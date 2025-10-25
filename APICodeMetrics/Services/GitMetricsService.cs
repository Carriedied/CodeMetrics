using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Services;

public class GitMetricsService : IGitMetricsCollector
{
    private readonly IProjectCollectorService _projectCollector;
    private readonly IRepositoryCollectorService _repositoryCollector;
    private readonly ILogger<GitMetricsService> _logger;

    public GitMetricsService(IProjectCollectorService projectCollector, IRepositoryCollectorService repositoryCollector, ILogger<GitMetricsService> logger)
    {
        _projectCollector = projectCollector;
        _repositoryCollector = repositoryCollector;
        _logger = logger;
    }

    public async Task<bool> CollectAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initiating full project collection process.");
        try
        {
            var result = await _projectCollector.CollectAllProjectsAsync(cancellationToken);
            _logger.LogInformation("Full project collection completed successfully.");
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Full project collection failed.");
            return false;
        }
    }

    public async Task<bool> CollectAllRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initiating full repositories collection process.");
        try
        {
            // Здесь можно получить список проектов и для каждого собрать репозитории
            // Но для простоты и демонстрации, предположим, что мы собираем репозитории для всех проектов
            // Это можно сделать через другой сервис или метод, если нужно.
            // В данном случае, для демонстрации, мы просто возвращаем успех.
            _logger.LogInformation("Full repositories collection initiated. This is a placeholder for future implementation that would iterate over projects.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Full repositories collection failed.");
            return false;
        }
    }
}