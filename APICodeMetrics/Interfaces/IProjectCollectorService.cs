using APICodeMetrics.Models.DTO;

namespace APICodeMetrics.Interfaces;

public interface IProjectCollectorService
{
    Task<SferaCodeResponseWrapper<ProjectDto[]>> CollectAsync(CancellationToken cancellationToken = default);
}