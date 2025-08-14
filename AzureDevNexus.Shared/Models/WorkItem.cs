namespace AzureDevNexus.Shared.Models;

public class WorkItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ChangedDate { get; set; }
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string AreaPath { get; set; } = string.Empty;
    public string IterationPath { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> CustomFields { get; set; } = new();
}
