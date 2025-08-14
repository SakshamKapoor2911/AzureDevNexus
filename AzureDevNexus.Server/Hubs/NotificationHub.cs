using Microsoft.AspNetCore.SignalR;
using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Server.Hubs
{
    public interface INotificationHub
    {
        Task SendNotification(string userId, NotificationMessage message);
        Task SendProjectUpdate(string projectId, ProjectUpdateMessage message);
        Task SendPipelineUpdate(string pipelineId, PipelineUpdateMessage message);
        Task SendWorkItemUpdate(string workItemId, WorkItemUpdateMessage message);
        Task SendGlobalNotification(GlobalNotificationMessage message);
    }

    public class NotificationHub : Hub, INotificationHub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} connected to notification hub", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} disconnected from notification hub", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, NotificationMessage message)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", message);
            _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message.Title);
        }

        public async Task SendProjectUpdate(string projectId, ProjectUpdateMessage message)
        {
            await Clients.All.SendAsync("ReceiveProjectUpdate", projectId, message);
            _logger.LogInformation("Project update sent for project {ProjectId}: {Message}", projectId, message.Message);
        }

        public async Task SendPipelineUpdate(string pipelineId, PipelineUpdateMessage message)
        {
            await Clients.All.SendAsync("ReceivePipelineUpdate", pipelineId, message);
            _logger.LogInformation("Pipeline update sent for pipeline {PipelineId}: {Status}", pipelineId, message.Status);
        }

        public async Task SendWorkItemUpdate(string workItemId, WorkItemUpdateMessage message)
        {
            await Clients.All.SendAsync("ReceiveWorkItemUpdate", workItemId, message);
            _logger.LogInformation("Work item update sent for work item {WorkItemId}: {Message}", workItemId, message.Message);
        }

        public async Task SendGlobalNotification(GlobalNotificationMessage message)
        {
            await Clients.All.SendAsync("ReceiveGlobalNotification", message);
            _logger.LogInformation("Global notification sent: {Message}", message.Message);
        }

        // Client methods
        public async Task JoinProjectGroup(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Project_{projectId}");
            _logger.LogInformation("Client joined project group {ProjectId}", projectId);
        }

        public async Task LeaveProjectGroup(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Project_{projectId}");
            _logger.LogInformation("Client left project group {ProjectId}", projectId);
        }

        public async Task JoinPipelineGroup(string pipelineId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Pipeline_{pipelineId}");
            _logger.LogInformation("Client joined pipeline group {PipelineId}", pipelineId);
        }

        public async Task LeavePipelineGroup(string pipelineId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Pipeline_{pipelineId}");
            _logger.LogInformation("Client left pipeline group {PipelineId}", pipelineId);
        }
    }
}
