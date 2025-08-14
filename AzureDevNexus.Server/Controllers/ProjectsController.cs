using Microsoft.AspNetCore.Mvc;
using AzureDevNexus.Shared.Interfaces;
using AzureDevNexus.Shared.Models;
using AzureDevNexus.Server.Attributes;

namespace AzureDevNexus.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IAzureDevOpsService _azureDevOpsService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IAzureDevOpsService azureDevOpsService,
        ILogger<ProjectsController> logger)
    {
        _azureDevOpsService = azureDevOpsService;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects
    /// </summary>
    /// <returns>List of all projects</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Project>>>> GetProjects()
    {
        try
        {
            var response = await _azureDevOpsService.GetProjectsAsync();
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, ApiResponse<List<Project>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get a specific project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Project>>> GetProject(string id)
    {
        try
        {
            var response = await _azureDevOpsService.GetProjectAsync(id);
            
            if (!response.Success)
                return NotFound(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
            return StatusCode(500, ApiResponse<Project>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get project metrics
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project metrics</returns>
    [HttpGet("{id}/metrics")]
    public async Task<ActionResult<ApiResponse<ProjectMetrics>>> GetProjectMetrics(string id)
    {
        try
        {
            var response = await _azureDevOpsService.GetProjectMetricsAsync(id);
            
            if (!response.Success)
                return NotFound(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project metrics for {ProjectId}", id);
            return StatusCode(500, ApiResponse<ProjectMetrics>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get project pipelines
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project pipelines</returns>
    [HttpGet("{id}/pipelines")]
    public async Task<ActionResult<ApiResponse<List<Pipeline>>>> GetProjectPipelines(string id)
    {
        try
        {
            var response = await _azureDevOpsService.GetPipelinesAsync(id);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project pipelines for {ProjectId}", id);
            return StatusCode(500, ApiResponse<List<Pipeline>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get project work items
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="type">Work item type filter</param>
    /// <returns>Project work items</returns>
    [HttpGet("{id}/workitems")]
    public async Task<ActionResult<ApiResponse<List<WorkItem>>>> GetProjectWorkItems(string id, [FromQuery] string? type = null)
    {
        try
        {
            var response = await _azureDevOpsService.GetWorkItemsAsync(id, type);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project work items for {ProjectId}", id);
            return StatusCode(500, ApiResponse<List<WorkItem>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get project repositories
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project repositories</returns>
    [HttpGet("{id}/repositories")]
    public async Task<ActionResult<ApiResponse<List<Repository>>>> GetProjectRepositories(string id)
    {
        try
        {
            var response = await _azureDevOpsService.GetRepositoriesAsync(id);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project repositories for {ProjectId}", id);
            return StatusCode(500, ApiResponse<List<Repository>>.CreateError("Internal server error"));
        }
    }
}
