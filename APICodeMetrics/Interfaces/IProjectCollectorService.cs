using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Interfaces;

public interface IProjectCollectorService
{
    Task<SferaCodeResponseWrapper<ProjectDto[]>> CollectAllProjectsAsync(CancellationToken cancellationToken = default);
}