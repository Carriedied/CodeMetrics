using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APICodeMetrics.Controllers;

[ApiController]
[Route("api/[controller]/collect")]
public class MetricsController(IGitMetricsCollector metricsCollector, ILogger<MetricsController> logger)
    : ControllerBase
{
    [HttpPost("projects")]
     public async Task<IActionResult> CollectProjects(CancellationToken cancellationToken)
     {
         logger.LogInformation("Received POST request to collect projects.");
         try
         {
                var success = await metricsCollector.CollectAllProjectsAsync(cancellationToken);
                if (success)
                {
                    logger.LogInformation("Projects collection completed successfully via POST.");
                    return Ok(new { Message = "Projects collection completed successfully." });
                }
                logger.LogWarning("Projects collection failed.");
                return StatusCode(500, new { Error = "Failed to collect projects." });
         }
         catch (Exception ex)
         {
             logger.LogError(ex, "An unhandled exception occurred during projects collection via POST.");
             return StatusCode(500, new { Error = "An internal error occurred while collecting projects." });
         }
     }

     [HttpPost("repositories")]
     public async Task<IActionResult> CollectRepositories(CancellationToken cancellationToken)
     {
         logger.LogInformation("Received POST request to collect repositories.");
         try
         {
             var success = await metricsCollector.CollectAllRepositoriesAsync(cancellationToken);
             if (success)
             {
                 logger.LogInformation("Repositories collection completed successfully via POST.");
                 return Ok(new { Message = "Repositories collection completed successfully." });
             }
             logger.LogWarning("Repositories collection failed.");
             return StatusCode(500, new { Error = "Failed to collect repositories." });
         }
         catch (Exception ex)
         {
             logger.LogError(ex, "An unhandled exception occurred during repositories collection via POST.");
             return StatusCode(500, new { Error = "An internal error occurred while collecting repositories." });
         }
     }
}