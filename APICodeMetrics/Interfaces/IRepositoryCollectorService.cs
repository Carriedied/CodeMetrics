using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Interfaces;

public interface IRepositoryCollectorService
{
    Task<SferaCodeResponseWrapper<RepositoryDto[]>> CollectAllRepositoriesForProjectAsync(string projectKey, CancellationToken cancellationToken = default);
}