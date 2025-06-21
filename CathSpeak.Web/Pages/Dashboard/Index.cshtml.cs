using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<ConversationDto> Conversations { get; set; } = new();
        public List<RoomDto> Rooms { get; set; } = new();
        public List<AccountDetailDto> Friends { get; set; } = new();

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                // Load conversations
                Conversations = await _apiService.GetAsync<List<ConversationDto>>("api/chat/conversations", token) ?? new();

                // Load rooms
                Rooms = await _apiService.GetAsync<List<RoomDto>>("api/rooms", token) ?? new();

                // Load friends
                Friends = await _apiService.GetAsync<List<AccountDetailDto>>("api/friendship/friends", token) ?? new();
            }
        }
    }
}