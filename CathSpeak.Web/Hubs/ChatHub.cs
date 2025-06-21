using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace CathSpeak.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task SendMessage(int conversationId, string message)
        {
            var username = Context.User?.Identity?.Name ?? "Unknown";

            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", new
                {
                    ConversationId = conversationId,
                    MessageContent = message,
                    MessageType = "Text",
                    CreateDate = DateTime.UtcNow,
                    Sender = new { Username = username }
                });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}