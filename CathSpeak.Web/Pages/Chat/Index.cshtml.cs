using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Chat
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<ConversationDto> Conversations { get; set; } = new();
        public List<AccountDetailDto> Friends { get; set; } = new();
        public ConversationDto? SelectedConversation { get; set; }
        public List<MessageDto> Messages { get; set; } = new();

        [BindProperty]
        public string NewMessage { get; set; } = string.Empty;

        [BindProperty]
        public int? SelectedConversationId { get; set; }

        public string? ErrorMessage { get; set; }

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync(int? conversationId = null)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Load conversations
                    Conversations = await _apiService.GetAsync<List<ConversationDto>>("api/chat/conversations", token) ?? new();

                    // Load friends for creating new conversations
                    Friends = await _apiService.GetAsync<List<AccountDetailDto>>("api/friendship/friends", token) ?? new();

                    // Select conversation
                    if (conversationId.HasValue && Conversations.Any())
                    {
                        SelectedConversation = Conversations.FirstOrDefault(c => c.ConversationId == conversationId.Value);
                        SelectedConversationId = conversationId.Value;
                    }
                    else if (Conversations.Any())
                    {
                        SelectedConversation = Conversations.First();
                        SelectedConversationId = SelectedConversation.ConversationId;
                    }

                    // Load messages for selected conversation
                    if (SelectedConversation != null)
                    {
                        Messages = await _apiService.GetAsync<List<MessageDto>>($"api/chat/conversations/{SelectedConversation.ConversationId}/messages", token) ?? new();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load chat data. Please try again.";
                }
            }
        }

        public async Task<IActionResult> OnPostSendMessageAsync()
        {
            if (!SelectedConversationId.HasValue || string.IsNullOrEmpty(NewMessage))
            {
                return RedirectToPage(new { conversationId = SelectedConversationId });
            }

            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var messageData = new
                    {
                        MessageContent = NewMessage.Trim(),
                        MessageType = "Text"
                    };

                    await _apiService.PostAsync<MessageDto>($"api/chat/conversations/{SelectedConversationId}/messages", messageData, token);
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Failed to send message. Please try again.";
                }
            }

            NewMessage = string.Empty;
            return RedirectToPage(new { conversationId = SelectedConversationId });
        }

        public async Task<IActionResult> OnPostCreateConversationAsync(int friendId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var conversationData = new { FriendId = friendId };
                    var newConversation = await _apiService.PostAsync<ConversationDto>("api/chat/conversations", conversationData, token);

                    if (newConversation != null)
                    {
                        return RedirectToPage(new { conversationId = newConversation.ConversationId });
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Failed to create conversation. Please try again.";
                }
            }

            return RedirectToPage();
        }
    }
}