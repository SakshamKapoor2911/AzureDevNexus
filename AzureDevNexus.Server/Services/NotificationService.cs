using AzureDevNexus.Server.Hubs;
using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Server.Services
{
    public interface INotificationService
    {
        Task SendUserNotificationAsync(string userId, NotificationMessage message);
        Task SendProjectUpdateAsync(string projectId, ProjectUpdateMessage message);
        Task SendPipelineUpdateAsync(string pipelineId, PipelineUpdateMessage message);
        Task SendWorkItemUpdateAsync(string workItemId, WorkItemUpdateMessage message);
        Task SendGlobalNotificationAsync(GlobalNotificationMessage message);
        Task SendPipelineStatusUpdateAsync(string pipelineId, string status, string? result = null, string? buildNumber = null);
        Task SendWorkItemStatusUpdateAsync(string workItemId, string title, WorkItemUpdateType type, string? oldValue = null, string? newValue = null);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationHub _notificationHub;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationHub notificationHub, ILogger<NotificationService> logger)
        {
            _notificationHub = notificationHub;
            _logger = logger;
        }

        public async Task SendUserNotificationAsync(string userId, NotificationMessage message)
        {
            try
            {
                await _notificationHub.SendNotification(userId, message);
                _logger.LogInformation("User notification sent to {UserId}: {Title}", userId, message.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user notification to {UserId}", userId);
            }
        }

        public async Task SendProjectUpdateAsync(string projectId, ProjectUpdateMessage message)
        {
            try
            {
                await _notificationHub.SendProjectUpdate(projectId, message);
                _logger.LogInformation("Project update notification sent for {ProjectId}: {Message}", projectId, message.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send project update notification for {ProjectId}", projectId);
            }
        }

        public async Task SendPipelineUpdateAsync(string pipelineId, PipelineUpdateMessage message)
        {
            try
            {
                await _notificationHub.SendPipelineUpdate(pipelineId, message);
                _logger.LogInformation("Pipeline update notification sent for {PipelineId}: {Status}", pipelineId, message.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send pipeline update notification for {PipelineId}", pipelineId);
            }
        }

        public async Task SendWorkItemUpdateAsync(string workItemId, WorkItemUpdateMessage message)
        {
            try
            {
                await _notificationHub.SendWorkItemUpdate(workItemId, message);
                _logger.LogInformation("Work item update notification sent for {WorkItemId}: {Message}", workItemId, message.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send work item update notification for {WorkItemId}", workItemId);
            }
        }

        public async Task SendGlobalNotificationAsync(GlobalNotificationMessage message)
        {
            try
            {
                await _notificationHub.SendGlobalNotification(message);
                _logger.LogInformation("Global notification sent: {Title}", message.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send global notification");
            }
        }

        public async Task SendPipelineStatusUpdateAsync(string pipelineId, string status, string? result = null, string? buildNumber = null)
        {
            var message = new PipelineUpdateMessage
            {
                PipelineId = pipelineId,
                Status = status,
                Result = result,
                BuildNumber = buildNumber,
                Timestamp = DateTime.UtcNow
            };

            await SendPipelineUpdateAsync(pipelineId, message);
        }

        public async Task SendWorkItemStatusUpdateAsync(string workItemId, string title, WorkItemUpdateType type, string? oldValue = null, string? newValue = null)
        {
            var message = new WorkItemUpdateMessage
            {
                WorkItemId = workItemId,
                WorkItemTitle = title,
                Type = type,
                Message = GetWorkItemUpdateMessage(type, oldValue, newValue),
                Timestamp = DateTime.UtcNow
            };

            await SendWorkItemUpdateAsync(workItemId, message);
        }

        private string GetWorkItemUpdateMessage(WorkItemUpdateType type, string? oldValue, string? newValue)
        {
            return type switch
            {
                WorkItemUpdateType.Created => "Work item created",
                WorkItemUpdateType.Updated => "Work item updated",
                WorkItemUpdateType.Deleted => "Work item deleted",
                WorkItemUpdateType.Assigned => $"Work item assigned to {newValue}",
                WorkItemUpdateType.Unassigned => "Work item unassigned",
                WorkItemUpdateType.StatusChanged => $"Status changed from {oldValue} to {newValue}",
                WorkItemUpdateType.PriorityChanged => $"Priority changed from {oldValue} to {newValue}",
                WorkItemUpdateType.CommentAdded => "Comment added",
                WorkItemUpdateType.AttachmentAdded => "Attachment added",
                _ => "Work item updated"
            };
        }
    }
}
