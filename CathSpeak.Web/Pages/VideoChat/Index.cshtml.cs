using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.VideoChat
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<VideoChatSessionDto> ActiveSessions { get; set; } = new();
        public List<RoomDto> AvailableRooms { get; set; } = new();
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
                ActiveSessions = await _apiService.GetAsync<List<VideoChatSessionDto>>("api/videochats/active", token) ?? new();
                AvailableRooms = await _apiService.GetAsync<List<RoomDto>>("api/rooms", token) ?? new();
                Friends = await _apiService.GetAsync<List<AccountDetailDto>>("api/friendship/friends", token) ?? new();
            }
        }

        public async Task<IActionResult> OnPostStartOneOnOneAsync(int friendId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var sessionData = new
                {
                    RoomId = 0, // For one-on-one, we might not need a room
                    SessionType = 1, // 1 for one-on-one
                    InvitedParticipants = new List<int> { friendId }
                };

                var session = await _apiService.PostAsync<VideoChatSessionDto>("api/videochats", sessionData, token);

                if (session != null)
                {
                    return RedirectToPage("/VideoChat/Session", new { sessionId = session.SessionId });
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostStartRoomSessionAsync(int roomId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var sessionData = new
                {
                    RoomId = roomId,
                    SessionType = 2, // 2 for room-based
                    InvitedParticipants = new List<int>()
                };

                var session = await _apiService.PostAsync<VideoChatSessionDto>("api/videochats", sessionData, token);

                if (session != null)
                {
                    return RedirectToPage("/VideoChat/Session", new { sessionId = session.SessionId });
                }
            }

            return RedirectToPage();
        }
    }
}