// using Azure.AI.OpenAI;
using AzureDevNexus.Shared.Interfaces;
using AzureDevNexus.Shared.Models;
using AzureDevNexus.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureDevNexus.Server.Services;

public class AICodeReviewService : IAICodeReviewService
{
    private readonly AzureDevNexusContext _context;
    private readonly ILogger<AICodeReviewService> _logger;
    private readonly IConfiguration _configuration;
    // Temporarily commented out until Azure OpenAI package issue is resolved
    // private readonly OpenAIClient? _openAIClient;

    public AICodeReviewService(
        AzureDevNexusContext context,
        ILogger<AICodeReviewService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        
        // Temporarily commented out until Azure OpenAI package issue is resolved
        // Initialize Azure OpenAI client if configuration is available
        // var endpoint = _configuration["AzureOpenAI:Endpoint"];
        // var apiKey = _configuration["AzureOpenAI:ApiKey"];
        
        // if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
        // {
        //     try
        //     {
        //         _openAIClient = new OpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
        //         _logger.LogInformation("Azure OpenAI client initialized successfully");
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogWarning(ex, "Failed to initialize Azure OpenAI client. Using mock responses only.");
        //         _openAIClient = null;
        //     }
        // }
    }

    public async Task<ApiResponse<CodeReviewResponse>> ReviewCodeAsync(CodeReviewRequest request)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            // Temporarily always use mock code review until Azure OpenAI package issue is resolved
            // if (_openAIClient != null)
            // {
            //     // Use Azure OpenAI Service for real code review
            //     var response = await PerformOpenAICodeReview(request);
            //     response.ProcessingTime = DateTime.UtcNow - startTime;
            //     
            //     // Save the review to database
            //     await SaveCodeReviewAsync(response);
            //     
            //     return ApiResponse<CodeReviewResponse>.CreateSuccess(response, "Code review completed using Azure OpenAI");
            // }
            // else
            // {
                // Fallback to mock code review for local development
                var response = await PerformMockCodeReview(request);
                response.ProcessingTime = DateTime.UtcNow - startTime;
                
                // Save the review to database
                await SaveCodeReviewAsync(response);
                
                return ApiResponse<CodeReviewResponse>.CreateSuccess(response, "Code review completed (mock response)");
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing code review");
            return ApiResponse<CodeReviewResponse>.CreateError("Failed to perform code review", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<CodeReviewResponse>>> GetCodeReviewHistoryAsync(string projectId, string? repositoryId = null)
    {
        try
        {
            var query = _context.CodeReviews.AsQueryable();
            
            // Filter by project if needed (you might need to add projectId to CodeReviewResponse model)
            var reviews = await query
                .OrderByDescending(r => r.ReviewDate)
                .Take(50)
                .ToListAsync();
            
            return ApiResponse<List<CodeReviewResponse>>.CreateSuccess(reviews, "Code review history retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving code review history");
            return ApiResponse<List<CodeReviewResponse>>.CreateError("Failed to retrieve code review history", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<CodeReviewResponse>> GetCodeReviewAsync(string reviewId)
    {
        try
        {
            var review = await _context.CodeReviews
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            
            if (review == null)
                return ApiResponse<CodeReviewResponse>.CreateError("Code review not found");
            
            return ApiResponse<CodeReviewResponse>.CreateSuccess(review, "Code review retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving code review {ReviewId}", reviewId);
            return ApiResponse<CodeReviewResponse>.CreateError("Failed to retrieve code review", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<bool>> DeleteCodeReviewAsync(string reviewId)
    {
        try
        {
            var review = await _context.CodeReviews
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            
            if (review == null)
                return ApiResponse<bool>.CreateError("Code review not found");

            _context.CodeReviews.Remove(review);
            await _context.SaveChangesAsync();
            
            return ApiResponse<bool>.CreateSuccess(true, "Code review deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting code review {ReviewId}", reviewId);
            return ApiResponse<bool>.CreateError("Failed to delete code review", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<CodeReviewResponse>>> ReviewMultipleFilesAsync(List<CodeReviewRequest> requests)
    {
        try
        {
            var responses = new List<CodeReviewResponse>();
            
            foreach (var request in requests)
            {
                var response = await ReviewCodeAsync(request);
                if (response.Success && response.Data != null)
                {
                    responses.Add(response.Data);
                }
            }
            
            return ApiResponse<List<CodeReviewResponse>>.CreateSuccess(responses, $"Code review completed for {responses.Count} files");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing multiple files");
            return ApiResponse<List<CodeReviewResponse>>.CreateError("Failed to review multiple files", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<CodeReviewResponse>> ReviewPullRequestAsync(string projectId, string repositoryId, string pullRequestId)
    {
        try
        {
            // This would integrate with Azure DevOps Git API to get PR details
            // For now, create a mock review
            var request = new CodeReviewRequest
            {
                CodeSnippet = "// Pull Request Code Review\n// This would contain the actual PR code",
                Language = "csharp",
                Context = $"Pull Request {pullRequestId} in repository {repositoryId}",
                ReviewType = "general",
                ProjectId = projectId,
                RepositoryId = repositoryId,
                FilePath = "pull-request-review"
            };
            
            return await ReviewCodeAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing pull request {PullRequestId}", pullRequestId);
            return ApiResponse<CodeReviewResponse>.CreateError("Failed to review pull request", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<Dictionary<string, object>>> GetReviewSettingsAsync()
    {
        try
        {
            var settings = new Dictionary<string, object>
            {
                ["DefaultModel"] = _configuration["AzureOpenAI:DefaultModel"] ?? "gpt-4",
                ["MaxCodeSnippetLength"] = 10000,
                ["MaxProcessingTimeSeconds"] = 30,
                ["EnableSecurityReview"] = true,
                ["EnablePerformanceReview"] = true,
                ["EnableBestPracticesReview"] = true,
                ["LanguageSpecificRules"] = new Dictionary<string, object>
                {
                    ["csharp"] = new { StyleGuide = "Microsoft", Framework = ".NET" },
                    ["javascript"] = new { StyleGuide = "Airbnb", Framework = "Node.js" },
                    ["python"] = new { StyleGuide = "PEP 8", Framework = "Python" }
                }
            };
            
            return ApiResponse<Dictionary<string, object>>.CreateSuccess(settings, "Review settings retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review settings");
            return ApiResponse<Dictionary<string, object>>.CreateError("Failed to retrieve review settings", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<bool>> UpdateReviewSettingsAsync(Dictionary<string, object> settings)
    {
        try
        {
            // In a real implementation, you would save these settings to configuration or database
            _logger.LogInformation("Review settings updated: {Settings}", string.Join(", ", settings.Select(kvp => $"{kvp.Key}={kvp.Value}")));
            
            return ApiResponse<bool>.CreateSuccess(true, "Review settings updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review settings");
            return ApiResponse<bool>.CreateError("Failed to update review settings", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<Dictionary<string, object>>> GetReviewAnalyticsAsync(string projectId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var reviews = await _context.CodeReviews
                .Where(r => r.ReviewDate >= startDate && r.ReviewDate <= endDate)
                .ToListAsync();
            
            var analytics = new Dictionary<string, object>
            {
                ["TotalReviews"] = reviews.Count,
                ["AverageQualityScore"] = reviews.Any() ? reviews.Average(r => r.QualityScore.OverallScore) : 0,
                ["ReviewsByType"] = reviews.GroupBy(r => r.ModelUsed).ToDictionary(g => g.Key, g => g.Count()),
                ["QualityTrend"] = reviews.OrderBy(r => r.ReviewDate).Select(r => new { Date = r.ReviewDate, Score = r.QualityScore.OverallScore }).ToList(),
                ["CommonIssues"] = reviews.SelectMany(r => r.Issues)
                    .GroupBy(i => i.Type)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
            
            return ApiResponse<Dictionary<string, object>>.CreateSuccess(analytics, "Review analytics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review analytics");
            return ApiResponse<Dictionary<string, object>>.CreateError("Failed to retrieve review analytics", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<CodeQualityScore>>> GetQualityTrendsAsync(string projectId, int days = 30)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var reviews = await _context.CodeReviews
                .Where(r => r.ReviewDate >= startDate)
                .OrderBy(r => r.ReviewDate)
                .ToListAsync();
            
            var trends = reviews.Select(r => new CodeQualityScore
            {
                OverallScore = r.QualityScore.OverallScore,
                ReadabilityScore = r.QualityScore.ReadabilityScore,
                MaintainabilityScore = r.QualityScore.MaintainabilityScore,
                PerformanceScore = r.QualityScore.PerformanceScore,
                SecurityScore = r.QualityScore.SecurityScore,
                Grade = r.QualityScore.Grade
            }).ToList();
            
            return ApiResponse<List<CodeQualityScore>>.CreateSuccess(trends, "Quality trends retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quality trends");
            return ApiResponse<List<CodeQualityScore>>.CreateError("Failed to retrieve quality trends", new List<string> { ex.Message });
        }
    }

    private async Task<CodeReviewResponse> PerformOpenAICodeReview(CodeReviewRequest request)
    {
        try
        {
            // Temporarily disabled Azure OpenAI integration until package issue is resolved
            // var systemPrompt = GenerateSystemPrompt(request.ReviewType);
            // var userPrompt = GenerateUserPrompt(request);
            
            // var chatCompletionsOptions = new ChatCompletionsOptions
            // {
            //     DeploymentName = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4",
            //     Messages =
            //     {
            //         new ChatMessage(ChatRole.System, systemPrompt),
            //         new ChatMessage(ChatRole.User, userPrompt)
            //     },
            //     MaxTokens = 2000,
            //     Temperature = 0.3f
            // };
            
            // var response = await _openAIClient!.GetChatCompletionsAsync(chatCompletionsOptions);
            // var completion = response.Value.Choices[0].Message.Content;
            
            // return ParseAIResponse(completion, request);
            
            // For now, return a mock response
            return await PerformMockCodeReview(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing OpenAI code review");
            throw;
        }
    }

    private async Task<CodeReviewResponse> PerformMockCodeReview(CodeReviewRequest request)
    {
        // Generate mock code review response for local development
        var reviewId = $"review-{Guid.NewGuid():N}";
        
        var issues = new List<CodeIssue>
        {
            new CodeIssue
            {
                Id = $"issue-{Guid.NewGuid():N}",
                Type = "warning",
                Severity = "medium",
                Message = "Consider using 'var' instead of explicit type declaration",
                Description = "Using 'var' can improve code readability in certain contexts",
                LineNumber = 1,
                ColumnNumber = 1,
                Code = "string name = \"value\";",
                Fix = "var name = \"value\";"
            },
            new CodeIssue
            {
                Id = $"issue-{Guid.NewGuid():N}",
                Type = "info",
                Severity = "low",
                Message = "Method could be made static",
                Description = "This method doesn't use instance members and could be static",
                LineNumber = 5,
                ColumnNumber = 1,
                Code = "public void ProcessData()",
                Fix = "public static void ProcessData()"
            }
        };
        
        var suggestions = new List<CodeSuggestion>
        {
            new CodeSuggestion
            {
                Id = $"suggestion-{Guid.NewGuid():N}",
                Type = "improvement",
                Title = "Add input validation",
                Description = "Consider adding null checks and input validation",
                Code = "public void ProcessInput(string input)",
                SuggestedCode = "public void ProcessInput(string? input)\n{\n    if (string.IsNullOrEmpty(input)) throw new ArgumentException(nameof(input));\n    // ... rest of method\n}",
                Reasoning = "Input validation improves code robustness and prevents runtime errors",
                Confidence = 0.9
            }
        };
        
        var qualityScore = new CodeQualityScore
        {
            OverallScore = 85.0,
            ReadabilityScore = 90.0,
            MaintainabilityScore = 80.0,
            PerformanceScore = 85.0,
            SecurityScore = 75.0,
            Grade = "B",
            Strengths = new List<string> { "Good naming conventions", "Consistent formatting", "Clear structure" },
            AreasForImprovement = new List<string> { "Add input validation", "Consider using 'var' where appropriate", "Add XML documentation" }
        };
        
        return new CodeReviewResponse
        {
            ReviewId = reviewId,
            Summary = "Code review completed with good overall quality. Some minor improvements suggested.",
            Issues = issues,
            Suggestions = suggestions,
            QualityScore = qualityScore,
            ReviewDate = DateTime.UtcNow,
            ModelUsed = "mock-model",
            ProcessingTime = TimeSpan.Zero
        };
    }

    private string GenerateSystemPrompt(string reviewType)
    {
        var basePrompt = "You are an expert code reviewer specializing in C# and .NET development. " +
                        "Analyze the provided code and provide constructive feedback focusing on:";
        
        var typeSpecificPrompt = reviewType switch
        {
            "security" => " security vulnerabilities, authentication issues, and data protection concerns.",
            "performance" => " performance bottlenecks, memory usage, and optimization opportunities.",
            "best-practices" => " coding standards, design patterns, and best practices.",
            _ => " code quality, maintainability, readability, and potential improvements."
        };
        
        return basePrompt + typeSpecificPrompt + 
               "\n\nProvide your response in the following JSON format:\n" +
               "{\n" +
               "  \"summary\": \"Brief overview of the review\",\n" +
               "  \"issues\": [{\"type\": \"error|warning|info\", \"severity\": \"high|medium|low\", \"message\": \"Issue description\", \"lineNumber\": 1, \"fix\": \"Suggested fix\"}],\n" +
               "  \"suggestions\": [{\"title\": \"Suggestion title\", \"description\": \"Description\", \"code\": \"Current code\", \"suggestedCode\": \"Improved code\", \"reasoning\": \"Why this improves the code\"}],\n" +
               "  \"qualityScore\": {\"overallScore\": 85.0, \"readabilityScore\": 90.0, \"maintainabilityScore\": 80.0, \"performanceScore\": 85.0, \"securityScore\": 75.0, \"grade\": \"A|B|C|D|F\", \"strengths\": [\"strength1\"], \"areasForImprovement\": [\"area1\"]}\n" +
               "}";
    }

    private string GenerateUserPrompt(CodeReviewRequest request)
    {
        return $"Please review the following {request.Language} code:\n\n" +
               $"Context: {request.Context}\n" +
               $"File Path: {request.FilePath}\n\n" +
               $"Code:\n```{request.Language}\n{request.CodeSnippet}\n```\n\n" +
               "Please provide a comprehensive code review following the specified format.";
    }

    private CodeReviewResponse ParseAIResponse(string aiResponse, CodeReviewRequest request)
    {
        try
        {
            // Parse the AI response JSON and convert to CodeReviewResponse
            // This is a simplified implementation - in production you'd want more robust JSON parsing
            
            var reviewId = $"review-{Guid.NewGuid():N}";
            
            // For now, return a basic response structure
            // In a real implementation, you'd parse the JSON response from AI
            return new CodeReviewResponse
            {
                ReviewId = reviewId,
                Summary = "AI code review completed successfully",
                Issues = new List<CodeIssue>(),
                Suggestions = new List<CodeSuggestion>(),
                QualityScore = new CodeQualityScore
                {
                    OverallScore = 80.0,
                    ReadabilityScore = 85.0,
                    MaintainabilityScore = 80.0,
                    PerformanceScore = 75.0,
                    SecurityScore = 80.0,
                    Grade = "B",
                    Strengths = new List<string> { "AI analysis completed" },
                    AreasForImprovement = new List<string> { "Review AI response for specific details" }
                },
                ReviewDate = DateTime.UtcNow,
                ModelUsed = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4",
                ProcessingTime = TimeSpan.Zero
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing AI response");
            
            // Return a fallback response
            return new CodeReviewResponse
            {
                ReviewId = $"review-{Guid.NewGuid():N}",
                Summary = "Code review completed with parsing error",
                Issues = new List<CodeIssue>(),
                Suggestions = new List<CodeSuggestion>(),
                QualityScore = new CodeQualityScore
                {
                    OverallScore = 0.0,
                    ReadabilityScore = 0.0,
                    MaintainabilityScore = 0.0,
                    PerformanceScore = 0.0,
                    SecurityScore = 0.0,
                    Grade = "F",
                    Strengths = new List<string>(),
                    AreasForImprovement = new List<string> { "AI response parsing failed" }
                },
                ReviewDate = DateTime.UtcNow,
                ModelUsed = "error",
                ProcessingTime = TimeSpan.Zero
            };
        }
    }

    private async Task SaveCodeReviewAsync(CodeReviewResponse response)
    {
        try
        {
            _context.CodeReviews.Add(response);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Code review saved to database: {ReviewId}", response.ReviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving code review to database");
            // Don't throw here as the main operation succeeded
        }
    }
}
