namespace AzureDevNexus.Shared.Models;

public class DashboardMetrics
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalPipelines { get; set; }
    public int FailedPipelineRuns { get; set; }
    public int TotalWorkItems { get; set; }
    public int OpenWorkItems { get; set; }
    public int TotalRepositories { get; set; }
    public int ActiveUsers { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ProjectMetrics
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int PipelineCount { get; set; }
    public int RepositoryCount { get; set; }
    public int WorkItemCount { get; set; }
    public int ActiveWorkItems { get; set; }
    public int CompletedWorkItems { get; set; }
    public double PipelineSuccessRate { get; set; }
    public DateTime LastActivity { get; set; }
}

public class PipelineMetrics
{
    public string PipelineId { get; set; } = string.Empty;
    public string PipelineName { get; set; } = string.Empty;
    public int TotalRuns { get; set; }
    public int SuccessfulRuns { get; set; }
    public int FailedRuns { get; set; }
    public int CancelledRuns { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public DateTime LastRun { get; set; }
}

public class RecentActivity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
