namespace APICodeMetrics.Interfaces;

public interface IGitMetricsCollector
{
    Task<bool> CollectAllProjectsAsync(CancellationToken cancellationToken = default);
    Task<bool> CollectAllRepositoriesAsync(CancellationToken cancellationToken = default);
}