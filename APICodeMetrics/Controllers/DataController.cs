using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APICodeMetrics.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController(
    IProjectCollectorService projectCollector,
    IRepositoryCollectorService repositoryCollector,
    ILogger<DataController> logger)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<SferaCodeResponseWrapper<ProjectDto[]>>> GetProjects(CancellationToken cancellationToken)
    {
        logger.LogInformation("Received GET request for projects data.");
        try
        {
            var data = await projectCollector.CollectAsync(cancellationToken);
            logger.LogInformation("Returning data for {ProjectCount} projects.", data.Data?.Length ?? 0);
            return Ok(data); // Возвращает JSON в нужном формате
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/data/projects");
            return StatusCode(500, new { error = "An internal error occurred while retrieving projects." });
        }
    }

    [HttpGet("{projectKey}/repositories")]
    public async Task<ActionResult<SferaCodeResponseWrapper<RepositoryDto[]>>> GetRepositories(string projectKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received GET request for repositories data for project: {ProjectKey}", projectKey);
        
        try
        {
            var data = await repositoryCollector.CollectAllRepositoriesForProjectAsync(projectKey, cancellationToken);
            logger.LogInformation("Returning data for {RepoCount} repositories in project {ProjectKey}.", data.Data?.Length ?? 0, projectKey);
            return Ok(data); // Возвращает JSON в нужном формате
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/data/projects/{projectKey}/repositories", projectKey);
            return StatusCode(500, new { error = "An internal error occurred while retrieving repositories." });
        }
    }
}