using Microsoft.AspNetCore.Mvc;
using AzureDevNexus.Shared.Interfaces;
using AzureDevNexus.Shared.Models;
using AzureDevNexus.Server.Attributes;

namespace AzureDevNexus.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AICodeReviewController : ControllerBase
{
    private readonly IAICodeReviewService _aiCodeReviewService;
    private readonly ILogger<AICodeReviewController> _logger;

    public AICodeReviewController(
        IAICodeReviewService aiCodeReviewService,
        ILogger<AICodeReviewController> logger)
    {
        _aiCodeReviewService = aiCodeReviewService;
        _logger = logger;
    }

    /// <summary>
    /// Review code using AI
    /// </summary>
    /// <param name="request">Code review request</param>
    /// <returns>AI code review response</returns>
    [HttpPost("review")]
    public async Task<ActionResult<ApiResponse<CodeReviewResponse>>> ReviewCode([FromBody] CodeReviewRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Code review request is required"));

            if (string.IsNullOrEmpty(request.CodeSnippet))
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Code snippet is required"));

            if (request.CodeSnippet.Length > 10000)
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Code snippet is too long (max 10,000 characters)"));

            var response = await _aiCodeReviewService.ReviewCodeAsync(request);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing AI code review");
            return StatusCode(500, ApiResponse<CodeReviewResponse>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get code review history
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="repositoryId">Optional repository ID</param>
    /// <returns>Code review history</returns>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<CodeReviewResponse>>>> GetReviewHistory(
        [FromQuery] string projectId,
        [FromQuery] string? repositoryId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest(ApiResponse<List<CodeReviewResponse>>.CreateError("Project ID is required"));

            var response = await _aiCodeReviewService.GetCodeReviewHistoryAsync(projectId, repositoryId);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving code review history for project {ProjectId}", projectId);
            return StatusCode(500, ApiResponse<List<CodeReviewResponse>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get a specific code review
    /// </summary>
    /// <param name="reviewId">Review ID</param>
    /// <returns>Code review details</returns>
    [HttpGet("{reviewId}")]
    public async Task<ActionResult<ApiResponse<CodeReviewResponse>>> GetReview(string reviewId)
    {
        try
        {
            if (string.IsNullOrEmpty(reviewId))
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Review ID is required"));

            var response = await _aiCodeReviewService.GetCodeReviewAsync(reviewId);
            
            if (!response.Success)
                return NotFound(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving code review {ReviewId}", reviewId);
            return StatusCode(500, ApiResponse<CodeReviewResponse>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Delete a code review
    /// </summary>
    /// <param name="reviewId">Review ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{reviewId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteReview(string reviewId)
    {
        try
        {
            if (string.IsNullOrEmpty(reviewId))
                return BadRequest(ApiResponse<bool>.CreateError("Review ID is required"));

            var response = await _aiCodeReviewService.DeleteCodeReviewAsync(reviewId);
            
            if (!response.Success)
                return NotFound(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting code review {ReviewId}", reviewId);
            return StatusCode(500, ApiResponse<bool>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Review multiple files
    /// </summary>
    /// <param name="requests">List of code review requests</param>
    /// <returns>Multiple code review responses</returns>
    [HttpPost("review-multiple")]
    public async Task<ActionResult<ApiResponse<List<CodeReviewResponse>>>> ReviewMultipleFiles(
        [FromBody] List<CodeReviewRequest> requests)
    {
        try
        {
            if (requests == null || !requests.Any())
                return BadRequest(ApiResponse<List<CodeReviewResponse>>.CreateError("Code review requests are required"));

            if (requests.Count > 10)
                return BadRequest(ApiResponse<List<CodeReviewResponse>>.CreateError("Maximum 10 files can be reviewed at once"));

            foreach (var request in requests)
            {
                if (string.IsNullOrEmpty(request.CodeSnippet))
                    return BadRequest(ApiResponse<List<CodeReviewResponse>>.CreateError("All code snippets are required"));

                if (request.CodeSnippet.Length > 10000)
                    return BadRequest(ApiResponse<List<CodeReviewResponse>>.CreateError("All code snippets must be under 10,000 characters"));
            }

            var response = await _aiCodeReviewService.ReviewMultipleFilesAsync(requests);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing multiple file code review");
            return StatusCode(500, ApiResponse<List<CodeReviewResponse>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Review a pull request
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="repositoryId">Repository ID</param>
    /// <param name="pullRequestId">Pull Request ID</param>
    /// <returns>Pull request code review</returns>
    [HttpPost("review-pr")]
    public async Task<ActionResult<ApiResponse<CodeReviewResponse>>> ReviewPullRequest(
        [FromQuery] string projectId,
        [FromQuery] string repositoryId,
        [FromQuery] string pullRequestId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Project ID is required"));

            if (string.IsNullOrEmpty(repositoryId))
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Repository ID is required"));

            if (string.IsNullOrEmpty(pullRequestId))
                return BadRequest(ApiResponse<CodeReviewResponse>.CreateError("Pull Request ID is required"));

            var response = await _aiCodeReviewService.ReviewPullRequestAsync(projectId, repositoryId, pullRequestId);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing pull request {PullRequestId}", pullRequestId);
            return StatusCode(500, ApiResponse<CodeReviewResponse>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get review settings
    /// </summary>
    /// <returns>Code review settings</returns>
    [HttpGet("settings")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> GetReviewSettings()
    {
        try
        {
            var response = await _aiCodeReviewService.GetReviewSettingsAsync();
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review settings");
            return StatusCode(500, ApiResponse<Dictionary<string, object>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Update review settings
    /// </summary>
    /// <param name="settings">Updated settings</param>
    /// <returns>Update result</returns>
    [HttpPut("settings")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateReviewSettings(
        [FromBody] Dictionary<string, object> settings)
    {
        try
        {
            if (settings == null || !settings.Any())
                return BadRequest(ApiResponse<bool>.CreateError("Settings are required"));

            var response = await _aiCodeReviewService.UpdateReviewSettingsAsync(settings);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review settings");
            return StatusCode(500, ApiResponse<bool>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get review analytics
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Review analytics</returns>
    [HttpGet("analytics")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> GetReviewAnalytics(
        [FromQuery] string projectId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest(ApiResponse<Dictionary<string, object>>.CreateError("Project ID is required"));

            if (startDate >= endDate)
                return BadRequest(ApiResponse<Dictionary<string, object>>.CreateError("Start date must be before end date"));

            if (endDate > DateTime.UtcNow)
                return BadRequest(ApiResponse<Dictionary<string, object>>.CreateError("End date cannot be in the future"));

            var response = await _aiCodeReviewService.GetReviewAnalyticsAsync(projectId, startDate, endDate);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review analytics for project {ProjectId}", projectId);
            return StatusCode(500, ApiResponse<Dictionary<string, object>>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Get quality trends
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="days">Number of days (default: 30)</param>
    /// <returns>Quality trends data</returns>
    [HttpGet("quality-trends")]
    public async Task<ActionResult<ApiResponse<List<CodeQualityScore>>>> GetQualityTrends(
        [FromQuery] string projectId,
        [FromQuery] int days = 30)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest(ApiResponse<List<CodeQualityScore>>.CreateError("Project ID is required"));

            if (days <= 0 || days > 365)
                return BadRequest(ApiResponse<List<CodeQualityScore>>.CreateError("Days must be between 1 and 365"));

            var response = await _aiCodeReviewService.GetQualityTrendsAsync(projectId, days);
            
            if (!response.Success)
                return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quality trends for project {ProjectId}", projectId);
            return StatusCode(500, ApiResponse<List<CodeQualityScore>>.CreateError("Internal server error"));
        }
    }
}
