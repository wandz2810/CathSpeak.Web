using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.VideoChat
{
    [Authorize]
    public class StartOneOnOneModel : PageModel
    {
        private readonly IApiService _apiService;

        [BindProperty(SupportsGet = true)]
        public int FriendId { get; set; }

        public AccountDetailDto? Friend { get; set; }
        public string? ErrorMessage { get; set; }

        public StartOneOnOneModel(IApiService apiService)
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
                    // Get friend details
                    Friend = await _apiService.GetAsync<AccountDetailDto>($"api/account/{FriendId}", token);

                    if (Friend == null)
                    {
                        ErrorMessage = "Friend not found.";
                        return Page();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to load friend details.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostStartCallAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var sessionData = new
                    {
                        RoomId = 0,
                        SessionType = 1, // One-on-one
                        InvitedParticipants = new List<int> { FriendId }
                    };

                    var session = await _apiService.PostAsync<VideoChatSessionDto>("api/videochats", sessionData, token);

                    if (session != null)
                    {
                        return RedirectToPage("/VideoChat/Session", new { sessionId = session.SessionId });
                    }

                    ErrorMessage = "Failed to start video call. Please try again.";
                }
                catch (Exception)
                {
                    ErrorMessage = "An error occurred while starting the video call.";
                }
            }

            await OnGetAsync();
            return Page();
        }
    }
}