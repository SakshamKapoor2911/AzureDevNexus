namespace AzureDevNexus.Shared.Models;

public class NotificationMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public string? UserId { get; set; }
    public string? ProjectId { get; set; }
    public string? PipelineId { get; set; }
    public string? WorkItemId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
}

public class ProjectUpdateMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ProjectUpdateType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

public class PipelineUpdateMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PipelineId { get; set; } = string.Empty;
    public string PipelineName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? BuildNumber { get; set; }
}

public class WorkItemUpdateMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkItemId { get; set; } = string.Empty;
    public string WorkItemTitle { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public WorkItemUpdateType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}

public class GlobalNotificationMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ActionUrl { get; set; }
    public bool RequiresAcknowledgment { get; set; }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    Pipeline,
    WorkItem,
    Project,
    System
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum ProjectUpdateType
{
    Created,
    Updated,
    Deleted,
    MemberAdded,
    MemberRemoved,
    PipelineAdded,
    PipelineRemoved,
    WorkItemAdded,
    WorkItemUpdated,
    WorkItemDeleted
}

public enum WorkItemUpdateType
{
    Created,
    Updated,
    Deleted,
    Assigned,
    Unassigned,
    StatusChanged,
    PriorityChanged,
    CommentAdded,
    AttachmentAdded
}
