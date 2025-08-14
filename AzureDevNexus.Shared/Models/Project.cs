namespace AzureDevNexus.Shared.Models;

public class Project
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public DateTime LastUpdateTime { get; set; }
    public Team DefaultTeam { get; set; } = new();
    public int RepositoryCount { get; set; }
    public int PipelineCount { get; set; }
    public int WorkItemCount { get; set; }
}

public class Team
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
