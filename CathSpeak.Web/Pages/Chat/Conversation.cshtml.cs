using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Chat
{
    [Authorize]
    public class ConversationModel : PageModel
    {
        private readonly IApiService _apiService;

        public ConversationDto? Conversation { get; set; }
        public List<MessageDto> Messages { get; set; } = new();

        [BindProperty]
        public string NewMessage { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int ConversationId { get; set; }

        public ConversationModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                // Get conversations to find the selected one
                var conversations = await _apiService.GetAsync<List<ConversationDto>>("api/chat/conversations", token) ?? new();
                Conversation = conversations.FirstOrDefault(c => c.ConversationId == ConversationId);

                if (Conversation != null)
                {
                    Messages = await _apiService.GetAsync<List<MessageDto>>($"api/chat/conversations/{ConversationId}/messages", token) ?? new();
                }
            }
        }

        public async Task<IActionResult> OnPostSendMessageAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(NewMessage))
            {
                var messageData = new
                {
                    MessageContent = NewMessage,
                    MessageType = "Text"
                };

                await _apiService.PostAsync<MessageDto>($"api/chat/conversations/{ConversationId}/messages", messageData, token);
                NewMessage = string.Empty;
            }

            return RedirectToPage(new { conversationId = ConversationId });
        }
    }
}