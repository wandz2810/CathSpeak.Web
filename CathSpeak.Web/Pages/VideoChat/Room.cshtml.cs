using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.VideoChat
{
    [Authorize]
    public class RoomModel : PageModel
    {
        private readonly IApiService _apiService;

        [BindProperty(SupportsGet = true)]
        public int RoomId { get; set; }

        public RoomDto? Room { get; set; }
        public string? ErrorMessage { get; set; }

        public RoomModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Get room details
                    Room = await _apiService.GetAsync<RoomDto>($"api/Rooms/{RoomId}", token);

                    if (Room == null)
                    {
                        ErrorMessage = "Room not found or you don't have access to it.";
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load room details.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostStartSessionAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var sessionData = new
                    {
                        RoomId = RoomId,
                        SessionType = 2, // 2 for room-based
                        InvitedParticipants = new List<int>()
                    };

                    var session = await _apiService.PostAsync<VideoChatSessionDto>("api/VideoChats", sessionData, token);

                    if (session != null)
                    {
                        return RedirectToPage("/VideoChat/Session", new { sessionId = session.SessionId });
                    }

                    ErrorMessage = "Failed to start video session. Please try again.";
                }
                catch (Exception)
                {
                    ErrorMessage = "An error occurred while starting the video session.";
                }
            }

            await OnGetAsync();
            return Page();
        }
    }
}