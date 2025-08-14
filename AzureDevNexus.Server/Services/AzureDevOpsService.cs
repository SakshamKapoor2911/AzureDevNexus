using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using AzureDevNexus.Shared.Interfaces;
using AzureDevNexus.Shared.Models;
using AzureDevNexus.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureDevNexus.Server.Services;

public class AzureDevOpsService : IAzureDevOpsService
{
    private readonly AzureDevNexusContext _context;
    private readonly ILogger<AzureDevOpsService> _logger;
    private readonly IConfiguration _configuration;
    private readonly VssConnection? _vssConnection;

    public AzureDevOpsService(
        AzureDevNexusContext context,
        ILogger<AzureDevOpsService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        
        // Initialize Azure DevOps connection if configuration is available
        var organizationUrl = _configuration["AzureDevOps:OrganizationUrl"];
        var personalAccessToken = _configuration["AzureDevOps:PersonalAccessToken"];
        
        // Temporarily disabled Azure DevOps connection until properly configured
        // if (!string.IsNullOrEmpty(organizationUrl) && !string.IsNullOrEmpty(personalAccessToken))
        // {
        //     try
        //     {
        //         var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
        //         _vssConnection = new VssConnection(new Uri(organizationUrl), credentials);
        //         _logger.LogInformation("Azure DevOps connection established successfully");
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogWarning(ex, "Failed to establish Azure DevOps connection. Using local data only.");
        //         _vssConnection = null;
        //     }
        // }
    }

    public async Task<ApiResponse<List<Project>>> GetProjectsAsync()
    {
        try
        {
            // Temporarily disabled Azure DevOps API integration until properly configured
            // if (_vssConnection != null)
            // {
            //     // Try to get projects from Azure DevOps API
            //     var projectClient = _vssConnection.GetClient<ProjectHttpClient>();
            //     var projects = await projectClient.GetProjectsAsync();
            //     
            //     var result = projects.Select(p => new Project
            //     {
            //         Id = p.Id.ToString(),
            //         Name = p.Name,
            //         Description = p.Description ?? string.Empty,
            //         Url = p.Url,
            //         State = p.State.ToString(),
            //         Visibility = p.Visibility.ToString(),
            //         LastUpdateTime = p.LastUpdateTime ?? DateTime.UtcNow,
            //         DefaultTeam = new Team { Id = "default", Name = "Default Team", Description = "Default team" },
            //         RepositoryCount = 0, // Will be populated separately
            //         PipelineCount = 0,   // Will be populated separately
            //         WorkItemCount = 0    // Will be populated separately
            //     }).ToList();

            //     return ApiResponse<List<Project>>.CreateSuccess(result, "Projects retrieved from Azure DevOps");
            // }
            // else
            // {
                // Fallback to local database
                var projects = await _context.Projects
                    .Include(p => p.DefaultTeam)
                    .ToListAsync();
                
                return ApiResponse<List<Project>>.CreateSuccess(projects, "Projects retrieved from local database");
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return ApiResponse<List<Project>>.CreateError("Failed to retrieve projects", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<Project>> GetProjectAsync(string projectId)
    {
        try
        {
            // Temporarily disabled Azure DevOps API integration until properly configured
            // if (_vssConnection != null)
            // {
            //     // Try to get project from Azure DevOps API
            //     var projectClient = _vssConnection.GetClient<ProjectHttpClient>();
            //     var project = await projectClient.GetProjectAsync(projectId);
            //     
            //     var result = new Project
            //     {
            //         Id = project.Id.ToString(),
            //         Name = project.Name,
            //         Description = project.Description ?? string.Empty,
            //         Url = project.Url,
            //         State = project.State.ToString(),
            //         Visibility = project.Visibility.ToString(),
            //         LastUpdateTime = project.LastUpdateTime ?? DateTime.UtcNow,
            //         DefaultTeam = new Team { Id = "default", Name = "Default Team", Description = "Default team" },
            //         RepositoryCount = 0,
            //         PipelineCount = 0,
            //         WorkItemCount = 0
            //     };

            //     return ApiResponse<Project>.CreateSuccess(result, "Project retrieved from Azure DevOps");
            // }
            // else
            // {
                // Fallback to local database
                var project = await _context.Projects
                    .Include(p => p.DefaultTeam)
                    .FirstOrDefaultAsync(p => p.Id == projectId);
                
                if (project == null)
                    return ApiResponse<Project>.CreateError("Project not found");
                
                return ApiResponse<Project>.CreateSuccess(project, "Project retrieved from local database");
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", projectId);
            return ApiResponse<Project>.CreateError("Failed to retrieve project", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<ProjectMetrics>> GetProjectMetricsAsync(string projectId)
    {
        try
        {
            var project = await GetProjectAsync(projectId);
            if (!project.Success || project.Data == null)
                return ApiResponse<ProjectMetrics>.CreateError("Project not found");

            var pipelineCount = await _context.Pipelines.CountAsync(p => p.ProjectId == projectId);
            var repositoryCount = await _context.Repositories.CountAsync(r => r.ProjectId == projectId);
            var workItemCount = await _context.WorkItems.CountAsync(w => w.ProjectId == projectId);
            var activeWorkItems = await _context.WorkItems.CountAsync(w => w.ProjectId == projectId && w.State == "Active");
            var completedWorkItems = await _context.WorkItems.CountAsync(w => w.ProjectId == projectId && w.State == "Closed");

            var pipelineSuccessRate = await CalculatePipelineSuccessRate(projectId);

            var metrics = new ProjectMetrics
            {
                ProjectId = projectId,
                ProjectName = project.Data.Name,
                PipelineCount = pipelineCount,
                RepositoryCount = repositoryCount,
                WorkItemCount = workItemCount,
                ActiveWorkItems = activeWorkItems,
                CompletedWorkItems = completedWorkItems,
                PipelineSuccessRate = pipelineSuccessRate,
                LastActivity = DateTime.UtcNow
            };

            return ApiResponse<ProjectMetrics>.CreateSuccess(metrics, "Project metrics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project metrics for {ProjectId}", projectId);
            return ApiResponse<ProjectMetrics>.CreateError("Failed to retrieve project metrics", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<Pipeline>>> GetPipelinesAsync(string? projectId = null)
    {
        try
        {
            // Temporarily disabled Azure DevOps API integration until properly configured
            // if (_vssConnection != null && !string.IsNullOrEmpty(projectId))
            // {
            //     // Try to get pipelines from Azure DevOps API
            //     var buildClient = _vssConnection.GetClient<BuildHttpClient>();
            //     var buildDefinitions = await buildClient.GetDefinitionsAsync(project: projectId);
            //     
            //     var result = buildDefinitions.Select(bd => new Pipeline
            //     {
            //         Id = bd.Id.ToString(),
            //         Name = bd.Name,
            //         ProjectId = projectId,
            //         ProjectName = bd.Project.Name,
            //         Type = "Build",
            //         Status = bd.QueueStatus.ToString(),
            //         LastRunDate = bd.LatestBuild?.StartTime ?? DateTime.UtcNow,
            //         LastRunStatus = bd.LatestBuild?.Status.ToString() ?? "Unknown",
            //         LastRunResult = bd.LatestBuild?.Result.ToString() ?? "Unknown",
            //         Url = bd.Url,
            //         RecentRuns = new List<PipelineRun>()
            //     }).ToList();

            //     return ApiResponse<List<Pipeline>>.CreateSuccess(result, "Pipelines retrieved from Azure DevOps");
            // }
            // else
            // {
                // Fallback to local database
                var query = _context.Pipelines.AsQueryable();
                if (!string.IsNullOrEmpty(projectId))
                    query = query.Where(p => p.ProjectId == projectId);

                var pipelines = await query
                    .Include(p => p.RecentRuns)
                    .ToListAsync();
                
                return ApiResponse<List<Pipeline>>.CreateSuccess(pipelines, "Pipelines retrieved from local database");
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipelines for project {ProjectId}", projectId);
            return ApiResponse<List<Pipeline>>.CreateError("Failed to retrieve pipelines", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<Pipeline>> GetPipelineAsync(string pipelineId)
    {
        try
        {
            var pipeline = await _context.Pipelines
                .Include(p => p.RecentRuns)
                .FirstOrDefaultAsync(p => p.Id == pipelineId);
            
            if (pipeline == null)
                return ApiResponse<Pipeline>.CreateError("Pipeline not found");
            
            return ApiResponse<Pipeline>.CreateSuccess(pipeline, "Pipeline retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline {PipelineId}", pipelineId);
            return ApiResponse<Pipeline>.CreateError("Failed to retrieve pipeline", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<PipelineRun>>> GetPipelineRunsAsync(string pipelineId)
    {
        try
        {
            var runs = await _context.PipelineRuns
                .Where(r => r.Id.StartsWith(pipelineId))
                .ToListAsync();
            
            return ApiResponse<List<PipelineRun>>.CreateSuccess(runs, "Pipeline runs retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline runs for {PipelineId}", pipelineId);
            return ApiResponse<List<PipelineRun>>.CreateError("Failed to retrieve pipeline runs", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<PipelineRun>> GetPipelineRunAsync(string pipelineId, string runId)
    {
        try
        {
            var run = await _context.PipelineRuns
                .FirstOrDefaultAsync(r => r.Id == runId);
            
            if (run == null)
                return ApiResponse<PipelineRun>.CreateError("Pipeline run not found");
            
            return ApiResponse<PipelineRun>.CreateSuccess(run, "Pipeline run retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline run {RunId}", runId);
            return ApiResponse<PipelineRun>.CreateError("Failed to retrieve pipeline run", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<string>> TriggerPipelineRunAsync(string pipelineId, Dictionary<string, string> parameters)
    {
        try
        {
            // Temporarily disabled Azure DevOps API integration until properly configured
            // if (_vssConnection != null)
            // {
            //     // Try to trigger pipeline in Azure DevOps
            //     var buildClient = _vssConnection.GetClient<BuildHttpClient>();
            //     var buildDefinition = await buildClient.GetDefinitionAsync(project: "AzureDevNexus", definitionId: int.Parse(pipelineId));
            //     
            //     var build = new Build
            //     {
            //         Definition = buildDefinition,
            //         Project = buildDefinition.Project,
            //         SourceBranch = "refs/heads/main",
            //         Parameters = System.Text.Json.JsonSerializer.Serialize(parameters)
            //     };

            //     var queuedBuild = await buildClient.QueueBuildAsync(build);
            //     
            //     return ApiResponse<string>.CreateSuccess(queuedBuild.Id.ToString(), "Pipeline triggered successfully in Azure DevOps");
            // }
            // else
            // {
                // Simulate pipeline trigger for local development
                var runId = $"run-{Guid.NewGuid():N}";
                var run = new PipelineRun
                {
                    Id = runId,
                    Name = $"Manual Run - {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                    Status = "Running",
                    Result = "Unknown",
                    StartTime = DateTime.UtcNow,
                    TriggeredBy = "Local User",
                    SourceBranch = "main",
                    SourceVersion = "local"
                };

                _context.PipelineRuns.Add(run);
                await _context.SaveChangesAsync();
                
                return ApiResponse<string>.CreateSuccess(runId, "Pipeline triggered successfully (local simulation)");
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering pipeline {PipelineId}", pipelineId);
            return ApiResponse<string>.CreateError("Failed to trigger pipeline", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<WorkItem>>> GetWorkItemsAsync(string? projectId = null, string? type = null)
    {
        try
        {
            var query = _context.WorkItems.AsQueryable();
            
            if (!string.IsNullOrEmpty(projectId))
                query = query.Where(w => w.ProjectId == projectId);
            
            if (!string.IsNullOrEmpty(type))
                query = query.Where(w => w.Type == type);

            var workItems = await query.ToListAsync();
            return ApiResponse<List<WorkItem>>.CreateSuccess(workItems, "Work items retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work items");
            return ApiResponse<List<WorkItem>>.CreateError("Failed to retrieve work items", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<WorkItem>> GetWorkItemAsync(string workItemId)
    {
        try
        {
            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            if (workItem == null)
                return ApiResponse<WorkItem>.CreateError("Work item not found");
            
            return ApiResponse<WorkItem>.CreateSuccess(workItem, "Work item retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work item {WorkItemId}", workItemId);
            return ApiResponse<WorkItem>.CreateError("Failed to retrieve work item", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<WorkItem>> CreateWorkItemAsync(WorkItem workItem)
    {
        try
        {
            workItem.Id = $"wi-{Guid.NewGuid():N}";
            workItem.CreatedDate = DateTime.UtcNow;
            
            _context.WorkItems.Add(workItem);
            await _context.SaveChangesAsync();
            
            return ApiResponse<WorkItem>.CreateSuccess(workItem, "Work item created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating work item");
            return ApiResponse<WorkItem>.CreateError("Failed to create work item", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<WorkItem>> UpdateWorkItemAsync(string workItemId, WorkItem workItem)
    {
        try
        {
            var existingWorkItem = await _context.WorkItems
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            if (existingWorkItem == null)
                return ApiResponse<WorkItem>.CreateError("Work item not found");

            existingWorkItem.Title = workItem.Title;
            existingWorkItem.Description = workItem.Description;
            existingWorkItem.Type = workItem.Type;
            existingWorkItem.State = workItem.State;
            existingWorkItem.Priority = workItem.Priority;
            existingWorkItem.AssignedTo = workItem.AssignedTo;
            existingWorkItem.ChangedDate = DateTime.UtcNow;
            existingWorkItem.Tags = workItem.Tags;
            existingWorkItem.CustomFields = workItem.CustomFields;

            await _context.SaveChangesAsync();
            
            return ApiResponse<WorkItem>.CreateSuccess(existingWorkItem, "Work item updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work item {WorkItemId}", workItemId);
            return ApiResponse<WorkItem>.CreateError("Failed to update work item", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<bool>> DeleteWorkItemAsync(string workItemId)
    {
        try
        {
            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            if (workItem == null)
                return ApiResponse<bool>.CreateError("Work item not found");

            _context.WorkItems.Remove(workItem);
            await _context.SaveChangesAsync();
            
            return ApiResponse<bool>.CreateSuccess(true, "Work item deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting work item {WorkItemId}", workItemId);
            return ApiResponse<bool>.CreateError("Failed to delete work item", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<Repository>>> GetRepositoriesAsync(string? projectId = null)
    {
        try
        {
            var query = _context.Repositories.AsQueryable();
            
            if (!string.IsNullOrEmpty(projectId))
                query = query.Where(r => r.ProjectId == projectId);

            var repositories = await query.ToListAsync();
            return ApiResponse<List<Repository>>.CreateSuccess(repositories, "Repositories retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repositories");
            return ApiResponse<List<Repository>>.CreateError("Failed to retrieve repositories", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<Repository>> GetRepositoryAsync(string repositoryId)
    {
        try
        {
            var repository = await _context.Repositories
                .FirstOrDefaultAsync(r => r.Id == repositoryId);
            
            if (repository == null)
                return ApiResponse<Repository>.CreateError("Repository not found");
            
            return ApiResponse<Repository>.CreateSuccess(repository, "Repository retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repository {RepositoryId}", repositoryId);
            return ApiResponse<Repository>.CreateError("Failed to retrieve repository", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<string>>> GetBranchesAsync(string repositoryId)
    {
        try
        {
            // For now, return default branches from local data
            var repository = await _context.Repositories
                .FirstOrDefaultAsync(r => r.Id == repositoryId);
            
            if (repository == null)
                return ApiResponse<List<string>>.CreateError("Repository not found");

            var branches = new List<string> { repository.DefaultBranch, "develop", "feature/new-feature" };
            return ApiResponse<List<string>>.CreateSuccess(branches, "Branches retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branches for repository {RepositoryId}", repositoryId);
            return ApiResponse<List<string>>.CreateError("Failed to retrieve branches", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<string>>> GetCommitsAsync(string repositoryId, string branch = "main")
    {
        try
        {
            // For now, return mock commit data
            var commits = new List<string>
            {
                "abc1234 - Initial commit",
                "def5678 - Add authentication system",
                "ghi9012 - Implement dashboard UI",
                "jkl3456 - Add pipeline monitoring",
                "mno7890 - Update documentation"
            };
            
            return ApiResponse<List<string>>.CreateSuccess(commits, "Commits retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving commits for repository {RepositoryId}", repositoryId);
            return ApiResponse<List<string>>.CreateError("Failed to retrieve commits", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<DashboardMetrics>> GetDashboardMetricsAsync()
    {
        try
        {
            var totalProjects = await _context.Projects.CountAsync();
            var activeProjects = await _context.Projects.CountAsync(p => p.State == "Active");
            var totalPipelines = await _context.Pipelines.CountAsync();
            var failedPipelineRuns = await _context.PipelineRuns.CountAsync(r => r.Result == "Failed");
            var totalWorkItems = await _context.WorkItems.CountAsync();
            var openWorkItems = await _context.WorkItems.CountAsync(w => w.State == "Active");
            var totalRepositories = await _context.Repositories.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);

            var metrics = new DashboardMetrics
            {
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                TotalPipelines = totalPipelines,
                FailedPipelineRuns = failedPipelineRuns,
                TotalWorkItems = totalWorkItems,
                OpenWorkItems = openWorkItems,
                TotalRepositories = totalRepositories,
                ActiveUsers = activeUsers,
                LastUpdated = DateTime.UtcNow
            };

            return ApiResponse<DashboardMetrics>.CreateSuccess(metrics, "Dashboard metrics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard metrics");
            return ApiResponse<DashboardMetrics>.CreateError("Failed to retrieve dashboard metrics", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<RecentActivity>>> GetRecentActivityAsync(int count = 10)
    {
        try
        {
            var activities = new List<RecentActivity>();
            
            // Get recent work item changes
            var recentWorkItems = await _context.WorkItems
                .OrderByDescending(w => w.ChangedDate ?? w.CreatedDate)
                .Take(count / 2)
                .ToListAsync();

            foreach (var workItem in recentWorkItems)
            {
                activities.Add(new RecentActivity
                {
                    Id = workItem.Id,
                    Type = "WorkItem",
                    Title = workItem.Title,
                    Description = $"Work item {workItem.State}",
                    User = workItem.AssignedTo,
                    Timestamp = workItem.ChangedDate ?? workItem.CreatedDate,
                    ProjectId = workItem.ProjectId,
                    ProjectName = workItem.ProjectName,
                    Status = workItem.State,
                    Url = $"#/workitems/{workItem.Id}"
                });
            }

            // Get recent pipeline runs
            var recentPipelineRuns = await _context.PipelineRuns
                .OrderByDescending(r => r.StartTime)
                .Take(count / 2)
                .ToListAsync();

            foreach (var run in recentPipelineRuns)
            {
                activities.Add(new RecentActivity
                {
                    Id = run.Id,
                    Type = "PipelineRun",
                    Title = run.Name,
                    Description = $"Pipeline run {run.Result}",
                    User = run.TriggeredBy,
                    Timestamp = run.StartTime,
                    ProjectId = "unknown",
                    ProjectName = "Unknown",
                    Status = run.Status,
                    Url = $"#/pipelines/{run.Id}"
                });
            }

            var orderedActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();

            return ApiResponse<List<RecentActivity>>.CreateSuccess(orderedActivities, "Recent activities retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activities");
            return ApiResponse<List<RecentActivity>>.CreateError("Failed to retrieve recent activities", new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<List<PipelineMetrics>>> GetPipelineMetricsAsync(string? projectId = null)
    {
        try
        {
            var query = _context.Pipelines.AsQueryable();
            
            if (!string.IsNullOrEmpty(projectId))
                query = query.Where(p => p.ProjectId == projectId);

            var pipelines = await query.ToListAsync();
            var metrics = new List<PipelineMetrics>();

            foreach (var pipeline in pipelines)
            {
                var runs = await _context.PipelineRuns
                    .Where(r => r.Id.StartsWith(pipeline.Id))
                    .ToListAsync();

                var totalRuns = runs.Count;
                var successfulRuns = runs.Count(r => r.Result == "Succeeded");
                var failedRuns = runs.Count(r => r.Result == "Failed");
                var cancelledRuns = runs.Count(r => r.Result == "Cancelled");
                var successRate = totalRuns > 0 ? (double)successfulRuns / totalRuns * 100 : 0;

                var averageDuration = runs.Any() 
                    ? TimeSpan.FromTicks((long)runs.Average(r => 
                        {
                            var finishTime = r.FinishTime ?? DateTime.UtcNow;
                            return (finishTime - r.StartTime).Ticks;
                        }))
                    : TimeSpan.Zero;

                metrics.Add(new PipelineMetrics
                {
                    PipelineId = pipeline.Id,
                    PipelineName = pipeline.Name,
                    TotalRuns = totalRuns,
                    SuccessfulRuns = successfulRuns,
                    FailedRuns = failedRuns,
                    CancelledRuns = cancelledRuns,
                    SuccessRate = successRate,
                    AverageDuration = averageDuration,
                    LastRun = runs.Any() ? runs.Max(r => r.StartTime) : DateTime.UtcNow
                });
            }

            return ApiResponse<List<PipelineMetrics>>.CreateSuccess(metrics, "Pipeline metrics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline metrics");
            return ApiResponse<List<PipelineMetrics>>.CreateError("Failed to retrieve pipeline metrics", new List<string> { ex.Message });
        }
    }

    private async Task<double> CalculatePipelineSuccessRate(string projectId)
    {
        try
        {
            var pipelineIds = await _context.Pipelines
                .Where(p => p.ProjectId == projectId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!pipelineIds.Any())
                return 0;

            var totalRuns = await _context.PipelineRuns
                .CountAsync(r => pipelineIds.Any(pid => r.Id.StartsWith(pid)));

            if (totalRuns == 0)
                return 0;

            var successfulRuns = await _context.PipelineRuns
                .CountAsync(r => pipelineIds.Any(pid => r.Id.StartsWith(pid)) && r.Result == "Succeeded");

            return (double)successfulRuns / totalRuns * 100;
        }
        catch
        {
            return 0;
        }
    }
}
