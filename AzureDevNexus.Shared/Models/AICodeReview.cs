namespace AzureDevNexus.Shared.Models;

public class CodeReviewRequest
{
    public string CodeSnippet { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string ReviewType { get; set; } = "general"; // general, security, performance, best-practices
    public string ProjectId { get; set; } = string.Empty;
    public string RepositoryId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

public class CodeReviewResponse
{
    public string ReviewId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<CodeIssue> Issues { get; set; } = new();
    public List<CodeSuggestion> Suggestions { get; set; } = new();
    public CodeQualityScore QualityScore { get; set; } = new();
    public DateTime ReviewDate { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
    public TimeSpan ProcessingTime { get; set; }
}

public class CodeIssue
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // error, warning, info
    public string Severity { get; set; } = string.Empty; // high, medium, low
    public string Message { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Fix { get; set; } = string.Empty;
}

public class CodeSuggestion
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // improvement, refactoring, optimization
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string SuggestedCode { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public class CodeQualityScore
{
    public double OverallScore { get; set; }
    public double ReadabilityScore { get; set; }
    public double MaintainabilityScore { get; set; }
    public double PerformanceScore { get; set; }
    public double SecurityScore { get; set; }
    public string Grade { get; set; } = string.Empty; // A, B, C, D, F
    public List<string> Strengths { get; set; } = new();
    public List<string> AreasForImprovement { get; set; } = new();
}
