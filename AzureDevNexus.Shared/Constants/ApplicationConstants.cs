namespace AzureDevNexus.Shared.Constants;

public static class ApplicationConstants
{
    public const string ApplicationName = "AzureDevNexus";
    public const string ApplicationVersion = "1.0.0";
    public const string ApplicationDescription = "Azure DevOps Management Platform";
    
    // API Endpoints
    public const string ApiBasePath = "/api";
    public const string AuthEndpoint = "/auth";
    public const string ProjectsEndpoint = "/projects";
    public const string PipelinesEndpoint = "/pipelines";
    public const string WorkItemsEndpoint = "/workitems";
    public const string RepositoriesEndpoint = "/repositories";
    public const string DashboardEndpoint = "/dashboard";
    public const string AICodeReviewEndpoint = "/ai/codereview";
    
    // Authentication
    public const string JwtBearerScheme = "Bearer";
    public const string AzureAdScheme = "AzureAD";
    public const int TokenExpirationMinutes = 60;
    public const int RefreshTokenExpirationDays = 7;
    
    // Pagination
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    
    // File Upload
    public const int MaxFileSizeMB = 10;
    public static readonly string[] AllowedFileExtensions = { ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".h", ".hpp" };
    
    // AI Code Review
    public const int MaxCodeSnippetLength = 10000;
    public const int MaxProcessingTimeSeconds = 30;
    public const string DefaultAIModel = "gpt-4";
    
    // Azure DevOps
    public const string AzureDevOpsApiVersion = "7.0";
    public const int MaxConcurrentRequests = 10;
    public const int RequestTimeoutSeconds = 30;
    
    // Caching
    public const int DefaultCacheExpirationMinutes = 15;
    public const int ProjectCacheExpirationMinutes = 30;
    public const int PipelineCacheExpirationMinutes = 5;
    
    // Error Messages
    public const string GenericErrorMessage = "An unexpected error occurred. Please try again.";
    public const string UnauthorizedMessage = "You are not authorized to perform this action.";
    public const string NotFoundMessage = "The requested resource was not found.";
    public const string ValidationErrorMessage = "The provided data is invalid.";
}
