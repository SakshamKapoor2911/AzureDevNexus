using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Shared.Interfaces;

public interface IAICodeReviewService
{
    Task<ApiResponse<CodeReviewResponse>> ReviewCodeAsync(CodeReviewRequest request);
    Task<ApiResponse<List<CodeReviewResponse>>> GetCodeReviewHistoryAsync(string projectId, string? repositoryId = null);
    Task<ApiResponse<CodeReviewResponse>> GetCodeReviewAsync(string reviewId);
    Task<ApiResponse<bool>> DeleteCodeReviewAsync(string reviewId);
    
    // Batch operations
    Task<ApiResponse<List<CodeReviewResponse>>> ReviewMultipleFilesAsync(List<CodeReviewRequest> requests);
    Task<ApiResponse<CodeReviewResponse>> ReviewPullRequestAsync(string projectId, string repositoryId, string pullRequestId);
    
    // Configuration
    Task<ApiResponse<Dictionary<string, object>>> GetReviewSettingsAsync();
    Task<ApiResponse<bool>> UpdateReviewSettingsAsync(Dictionary<string, object> settings);
    
    // Analytics
    Task<ApiResponse<Dictionary<string, object>>> GetReviewAnalyticsAsync(string projectId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<CodeQualityScore>>> GetQualityTrendsAsync(string projectId, int days = 30);
}
