namespace AzureDevNexus.Shared.Enums;

public enum ProjectState
{
    Active,
    Inactive,
    Archived,
    Deleted
}

public enum ProjectVisibility
{
    Private,
    Public,
    Internal
}

public enum PipelineStatus
{
    Idle,
    Running,
    Paused,
    Disabled
}

public enum PipelineResult
{
    Succeeded,
    Failed,
    Cancelled,
    PartiallySucceeded,
    Skipped
}

public enum WorkItemType
{
    Bug,
    Task,
    UserStory,
    Feature,
    Epic,
    Issue,
    TestCase
}

public enum WorkItemState
{
    New,
    Active,
    Resolved,
    Closed,
    Removed
}

public enum WorkItemPriority
{
    Critical,
    High,
    Medium,
    Low
}

public enum RepositoryType
{
    Git,
    TFVC
}

public enum CodeReviewType
{
    General,
    Security,
    Performance,
    BestPractices,
    Accessibility,
    Documentation
}

public enum CodeIssueSeverity
{
    High,
    Medium,
    Low,
    Info
}
