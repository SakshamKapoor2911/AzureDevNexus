using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Shared.Interfaces;

public interface IAzureDevOpsService
{
    // Project operations
    Task<ApiResponse<List<Project>>> GetProjectsAsync();
    Task<ApiResponse<Project>> GetProjectAsync(string projectId);
    Task<ApiResponse<ProjectMetrics>> GetProjectMetricsAsync(string projectId);
    
    // Pipeline operations
    Task<ApiResponse<List<Pipeline>>> GetPipelinesAsync(string? projectId = null);
    Task<ApiResponse<Pipeline>> GetPipelineAsync(string pipelineId);
    Task<ApiResponse<List<PipelineRun>>> GetPipelineRunsAsync(string pipelineId);
    Task<ApiResponse<PipelineRun>> GetPipelineRunAsync(string pipelineId, string runId);
    Task<ApiResponse<string>> TriggerPipelineRunAsync(string pipelineId, Dictionary<string, string> parameters);
    
    // Work Item operations
    Task<ApiResponse<List<WorkItem>>> GetWorkItemsAsync(string? projectId = null, string? type = null);
    Task<ApiResponse<WorkItem>> GetWorkItemAsync(string workItemId);
    Task<ApiResponse<WorkItem>> CreateWorkItemAsync(WorkItem workItem);
    Task<ApiResponse<WorkItem>> UpdateWorkItemAsync(string workItemId, WorkItem workItem);
    Task<ApiResponse<bool>> DeleteWorkItemAsync(string workItemId);
    
    // Repository operations
    Task<ApiResponse<List<Repository>>> GetRepositoriesAsync(string? projectId = null);
    Task<ApiResponse<Repository>> GetRepositoryAsync(string repositoryId);
    Task<ApiResponse<List<string>>> GetBranchesAsync(string repositoryId);
    Task<ApiResponse<List<string>>> GetCommitsAsync(string repositoryId, string branch = "main");
    
    // Dashboard operations
    Task<ApiResponse<DashboardMetrics>> GetDashboardMetricsAsync();
    Task<ApiResponse<List<RecentActivity>>> GetRecentActivityAsync(int count = 10);
    Task<ApiResponse<List<PipelineMetrics>>> GetPipelineMetricsAsync(string? projectId = null);
}
