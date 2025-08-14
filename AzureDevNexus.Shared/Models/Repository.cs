namespace AzureDevNexus.Shared.Models;

public class Repository
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsFork { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public int CommitCount { get; set; }
    public int BranchCount { get; set; }
    public int PullRequestCount { get; set; }
    public RepositoryPermissions Permissions { get; set; } = new();
}

public class RepositoryPermissions
{
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
}
