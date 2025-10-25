using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using APICodeMetrics.Services;
using Microsoft.AspNetCore.Mvc;

namespace APICodeMetrics.Controllers;

[ApiController]
[Route("api/[controller]/collect")]
public class MetricsController : ControllerBase
{
    private readonly DataCollectionOrchestrator _orchestrator;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(DataCollectionOrchestrator orchestrator, ILogger<MetricsController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [HttpPost("collect/all")]
    public async Task<IActionResult> CollectAllData(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request to collect all data.");
        try
        {
            var success = await _orchestrator.CollectAllDataAsync(cancellationToken);
            if (success)
            {
                _logger.LogInformation("All data collection completed successfully via POST.");
                return Ok(new { Message = "All data collection completed successfully." });
            }
            _logger.LogWarning("All data collection failed.");
            return StatusCode(500, new { Error = "Failed to collect all data." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during all data collection via POST.");
            return StatusCode(500, new { Error = "An internal error occurred while collecting all data." });
        }
    }
}