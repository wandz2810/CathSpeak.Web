using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace CathSpeak.Web.Hubs
{
    [Authorize]
    public class VideoChatHub : Hub
    {
        public async Task JoinSession(int sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session_{sessionId}");

            var username = Context.User?.Identity?.Name ?? "Unknown";
            await Clients.Group($"session_{sessionId}")
                .SendAsync("UserJoined", sessionId, username);
        }

        public async Task LeaveSession(int sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session_{sessionId}");

            var username = Context.User?.Identity?.Name ?? "Unknown";
            await Clients.Group($"session_{sessionId}")
                .SendAsync("UserLeft", sessionId, username);
        }

        public async Task SendSessionMessage(int sessionId, string message)
        {
            var username = Context.User?.Identity?.Name ?? "Unknown";

            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveSessionMessage", username, message, DateTime.UtcNow);
        }

        public async Task SendOffer(int sessionId, string targetUserId, string offer)
        {
            var username = Context.User?.Identity?.Name ?? "Unknown";
            
            // Get connection ID of target user (you'll need to track this when users join)
            var connectionId = GetConnectionIdForUser(targetUserId);
            
            if (!string.IsNullOrEmpty(connectionId))
            {
                // Send directly to target user
                await Clients.Client(connectionId).SendAsync("ReceiveOffer", username, offer);
            }
            else
            {
                // Fall back to group broadcast if we can't find a specific connection
                await Clients.Group($"session_{sessionId}").SendAsync("ReceiveOffer", username, offer);
            }
        }

        // Helper method to get connection ID for user
        private string GetConnectionIdForUser(string userId)
        {
            // You need to implement connection tracking - this is just a placeholder
            // Consider using a static dictionary or dependency injection with a service
            // that maps user IDs to connection IDs
            return null;
        }

        public async Task SendAnswer(int sessionId, string targetUserId, string answer)
        {
            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveAnswer", Context.User?.Identity?.Name, answer);
        }

        public async Task SendIceCandidate(int sessionId, string targetUserId, string candidate)
        {
            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveIceCandidate", Context.User?.Identity?.Name, candidate);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Find which sessions this user was part of
            // This would require tracking user sessions
            var username = Context.User?.Identity?.Name;
            var sessionIds = GetSessionsForUser(username);
            
            foreach (var sessionId in sessionIds)
            {
                // Notify other participants that this user left
                await Clients.Group($"session_{sessionId}")
                    .SendAsync("UserLeft", sessionId, username);
            }
            
            // Call the base implementation
            await base.OnDisconnectedAsync(exception);
        }

        private List<int> GetSessionsForUser(string username)
        {
            // Implement logic to retrieve the sessions this user was part of
            // This could be stored in a static dictionary or database
            return new List<int>();
        }
    }
}