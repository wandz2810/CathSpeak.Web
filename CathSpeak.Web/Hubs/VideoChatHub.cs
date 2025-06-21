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

        public async Task SendOffer(int sessionId, string targetUser, string offer)
        {
            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveOffer", Context.User?.Identity?.Name, targetUser, offer);
        }

        public async Task SendAnswer(int sessionId, string targetUser, string answer)
        {
            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveAnswer", Context.User?.Identity?.Name, targetUser, answer);
        }

        public async Task SendIceCandidate(int sessionId, string targetUser, string candidate)
        {
            await Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveIceCandidate", Context.User?.Identity?.Name, targetUser, candidate);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Handle user disconnection
            await base.OnDisconnectedAsync(exception);
        }
    }
}