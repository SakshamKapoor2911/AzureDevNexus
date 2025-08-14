using Microsoft.EntityFrameworkCore;
using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Server.Data;

public class AzureDevNexusContext : DbContext
{
    public AzureDevNexusContext(DbContextOptions<AzureDevNexusContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Pipeline> Pipelines { get; set; }
    public DbSet<PipelineRun> PipelineRuns { get; set; }
    public DbSet<WorkItem> WorkItems { get; set; }
    public DbSet<Repository> Repositories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<CodeReviewResponse> CodeReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Url).HasMaxLength(500);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Visibility).HasMaxLength(50);
            
            entity.HasOne(e => e.DefaultTeam)
                  .WithMany()
                  .HasForeignKey("DefaultTeamId")
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Team entity
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // Configure Pipeline entity
        modelBuilder.Entity<Pipeline>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ProjectId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.LastRunStatus).HasMaxLength(50);
            entity.Property(e => e.LastRunResult).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(500);
            
            entity.HasMany(e => e.RecentRuns)
                  .WithOne()
                  .HasForeignKey("PipelineId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PipelineRun entity
        modelBuilder.Entity<PipelineRun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.TriggeredBy).HasMaxLength(200);
            entity.Property(e => e.SourceBranch).HasMaxLength(100);
            entity.Property(e => e.SourceVersion).HasMaxLength(50);
        });

        // Configure WorkItem entity
        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.AssignedTo).HasMaxLength(200);
            entity.Property(e => e.ProjectId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.AreaPath).HasMaxLength(500);
            entity.Property(e => e.IterationPath).HasMaxLength(500);
        });

        // Configure Repository entity
        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ProjectId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);
            entity.Property(e => e.DefaultBranch).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.AzureAdId).HasMaxLength(100);
        });

        // Configure UserProfile entity
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.TimeZone).HasMaxLength(50);
            entity.Property(e => e.Language).HasMaxLength(10);
        });

        // Configure CodeReviewResponse entity
        modelBuilder.Entity<CodeReviewResponse>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.ReviewId).HasMaxLength(50);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.Property(e => e.ModelUsed).HasMaxLength(100);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Teams
        var defaultTeam = new Team
        {
            Id = "team-001",
            Name = "Default Team",
            Description = "Default team for new projects"
        };

        // Seed Projects
        var projects = new[]
        {
            new Project
            {
                Id = "proj-001",
                Name = "AzureDevNexus",
                Description = "Azure DevOps Management Platform",
                Url = "https://dev.azure.com/organization/AzureDevNexus",
                State = "Active",
                Visibility = "Private",
                LastUpdateTime = DateTime.UtcNow,
                DefaultTeam = defaultTeam,
                RepositoryCount = 3,
                PipelineCount = 5,
                WorkItemCount = 25
            },
            new Project
            {
                Id = "proj-002",
                Name = "Sample Project",
                Description = "A sample project for demonstration",
                Url = "https://dev.azure.com/organization/SampleProject",
                State = "Active",
                Visibility = "Private",
                LastUpdateTime = DateTime.UtcNow.AddDays(-1),
                DefaultTeam = defaultTeam,
                RepositoryCount = 2,
                PipelineCount = 3,
                WorkItemCount = 15
            }
        };

        // Seed Pipelines
        var pipelines = new[]
        {
            new Pipeline
            {
                Id = "pipe-001",
                Name = "Build Pipeline",
                ProjectId = "proj-001",
                ProjectName = "AzureDevNexus",
                Type = "Build",
                Status = "Idle",
                LastRunDate = DateTime.UtcNow.AddHours(-2),
                LastRunStatus = "Completed",
                LastRunResult = "Succeeded",
                Url = "https://dev.azure.com/organization/AzureDevNexus/_build?definitionId=1"
            },
            new Pipeline
            {
                Id = "pipe-002",
                Name = "Deploy Pipeline",
                ProjectId = "proj-001",
                ProjectName = "AzureDevNexus",
                Type = "Release",
                Status = "Idle",
                LastRunDate = DateTime.UtcNow.AddDays(-1),
                LastRunStatus = "Completed",
                LastRunResult = "Succeeded",
                Url = "https://dev.azure.com/organization/AzureDevNexus/_release?definitionId=1"
            }
        };

        // Seed Work Items
        var workItems = new[]
        {
            new WorkItem
            {
                Id = "wi-001",
                Title = "Implement Authentication System",
                Description = "Set up Azure AD authentication with JWT tokens",
                Type = "Task",
                State = "Active",
                Priority = "High",
                AssignedTo = "developer@company.com",
                ProjectId = "proj-001",
                ProjectName = "AzureDevNexus",
                AreaPath = "AzureDevNexus\\Authentication",
                IterationPath = "AzureDevNexus\\Sprint 1",
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                Tags = new List<string> { "authentication", "security", "azure-ad" }
            },
            new WorkItem
            {
                Id = "wi-002",
                Title = "Create Dashboard UI",
                Description = "Design and implement the main dashboard interface",
                Type = "UserStory",
                State = "Active",
                Priority = "Medium",
                AssignedTo = "developer@company.com",
                ProjectId = "proj-001",
                ProjectName = "AzureDevNexus",
                AreaPath = "AzureDevNexus\\UI",
                IterationPath = "AzureDevNexus\\Sprint 1",
                CreatedDate = DateTime.UtcNow.AddDays(-3),
                Tags = new List<string> { "ui", "dashboard", "blazor" }
            }
        };

        // Seed Repositories
        var repositories = new[]
        {
            new Repository
            {
                Id = "repo-001",
                Name = "AzureDevNexus",
                ProjectId = "proj-001",
                ProjectName = "AzureDevNexus",
                Url = "https://dev.azure.com/organization/AzureDevNexus/_git/AzureDevNexus",
                DefaultBranch = "main",
                Type = "Git",
                IsFork = false,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                LastUpdatedDate = DateTime.UtcNow.AddHours(-1),
                CommitCount = 45,
                BranchCount = 3,
                PullRequestCount = 8
            }
        };

        // Seed Users
        var users = new[]
        {
            new User
            {
                Id = "user-001",
                Username = "developer",
                Email = "developer@company.com",
                DisplayName = "John Developer",
                Role = "Developer",
                AzureAdId = "azure-ad-id-001",
                CreatedDate = DateTime.UtcNow.AddDays(-30),
                LastLoginDate = DateTime.UtcNow.AddHours(-2),
                IsActive = true,
                Permissions = new List<string> { "read", "write", "delete" }
            }
        };

        modelBuilder.Entity<Team>().HasData(defaultTeam);
        modelBuilder.Entity<Project>().HasData(projects);
        modelBuilder.Entity<Pipeline>().HasData(pipelines);
        modelBuilder.Entity<WorkItem>().HasData(workItems);
        modelBuilder.Entity<Repository>().HasData(repositories);
        modelBuilder.Entity<User>().HasData(users);
    }
}
