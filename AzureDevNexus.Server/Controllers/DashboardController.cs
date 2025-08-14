using Microsoft.AspNetCore.Mvc;
using AzureDevNexus.Shared.Interfaces;
using AzureDevNexus.Shared.Models;
using AzureDevNexus.Server.Attributes;

namespace AzureDevNexus.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IAzureDevOpsService _azureDevOpsService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IAzureDevOpsService azureDevOpsService,
        ILogger<DashboardController> logger)
    {
        _azureDevOpsService = azureDevOpsService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard metrics
    /// </summary>
    /// <returns>Dashboard metrics overview</returns>
    [HttpGet("metrics")]
    public async Task<ActionResult<ApiResponse<DashboardMetrics>>> GetMetrics()
    {
        try
        {
            var response = await _azureDevOpsService.GetDashboardMetricsAsync();
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard metrics");
            return StatusCode(500, ApiResponse<DashboardMetrics>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get recent activity
    /// </summary>
    /// <param name="count">Number of activities to retrieve (default: 10)</param>
    /// <returns>Recent activity list</returns>
    [HttpGet("recent-activity")]
    public async Task<ActionResult<ApiResponse<List<RecentActivity>>>> GetRecentActivity([FromQuery] int count = 10)
    {
        try
        {
            if (count <= 0 || count > 100)
                return BadRequest(ApiResponse<List<RecentActivity>>.CreateError("Count must be between 1 and 100"));

            var response = await _azureDevOpsService.GetRecentActivityAsync(count);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activity");
            return StatusCode(500, ApiResponse<List<RecentActivity>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get pipeline metrics
    /// </summary>
    /// <param name="projectId">Optional project ID filter</param>
    /// <returns>Pipeline metrics</returns>
    [HttpGet("pipeline-metrics")]
    public async Task<ActionResult<ApiResponse<List<PipelineMetrics>>>> GetPipelineMetrics([FromQuery] string? projectId = null)
    {
        try
        {
            var response = await _azureDevOpsService.GetPipelineMetricsAsync(projectId);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline metrics");
            return StatusCode(500, ApiResponse<List<PipelineMetrics>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get project metrics summary
    /// </summary>
    /// <returns>Project metrics summary</returns>
    [HttpGet("project-summary")]
    public async Task<ActionResult<ApiResponse<object>>> GetProjectSummary()
    {
        try
        {
            var projectsResponse = await _azureDevOpsService.GetProjectsAsync();
            if (!projectsResponse.Success || projectsResponse.Data == null)
                return BadRequest(projectsResponse);

            var summary = new
            {
                TotalProjects = projectsResponse.Data.Count,
                ActiveProjects = projectsResponse.Data.Count(p => p.State == "Active"),
                ProjectsByVisibility = projectsResponse.Data.GroupBy(p => p.Visibility)
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentProjects = projectsResponse.Data
                    .OrderByDescending(p => p.LastUpdateTime)
                    .Take(5)
                    .Select(p => new { p.Id, p.Name, p.State, p.LastUpdateTime })
            };

            return Ok(ApiResponse<object>.CreateSuccess(summary, "Project summary retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project summary");
            return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    /// <returns>System health information</returns>
    [HttpGet("health")]
    public ActionResult<ApiResponse<object>> GetHealth()
    {
        try
        {
            var health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Services = new
                {
                    Database = "Connected",
                    AzureDevOps = "Available",
                    AzureOpenAI = "Available"
                }
            };

            return Ok(ApiResponse<object>.CreateSuccess(health, "System health check completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking system health");
            return StatusCode(500, ApiResponse<object>.CreateError("System health check failed"));
        }
    }
}
