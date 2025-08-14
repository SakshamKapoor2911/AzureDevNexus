namespace AzureDevNexus.Shared.Models;

public class Pipeline
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastRunDate { get; set; }
    public string LastRunStatus { get; set; } = string.Empty;
    public string LastRunResult { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<PipelineRun> RecentRuns { get; set; } = new();
}

public class PipelineRun
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? FinishTime { get; set; }
    public string TriggeredBy { get; set; } = string.Empty;
    public string SourceBranch { get; set; } = string.Empty;
    public string SourceVersion { get; set; } = string.Empty;
}
