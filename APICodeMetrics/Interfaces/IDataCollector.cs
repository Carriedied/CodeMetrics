namespace APICodeMetrics.Interfaces;

public interface IDataCollector<T>
{
    Task<T> CollectAsync(object context, CancellationToken cancellationToken = default);
}