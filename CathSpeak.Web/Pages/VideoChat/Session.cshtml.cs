using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.VideoChat
{
    [Authorize]
    public class SessionModel : PageModel
    {
        private readonly IApiService _apiService;

        public VideoChatSessionDto? Session { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SessionId { get; set; }

        public SessionModel(IApiService apiService)
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
                    Session = await _apiService.GetAsync<VideoChatSessionDto>($"api/videochats/{SessionId}", token);

                    if (Session == null)
                    {
                        ErrorMessage = "Session not found or you don't have access to it.";
                        return Page();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to load session details.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostJoinAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    await _apiService.PostAsync<dynamic>($"api/videochats/{SessionId}/join", new { }, token);
                    return RedirectToPage(new { sessionId = SessionId });
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to join session.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    await _apiService.PostAsync<dynamic>($"api/videochats/{SessionId}/leave", new { }, token);
                    return RedirectToPage("/VideoChat/Index");
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to leave session.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEndAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    await _apiService.PostAsync<dynamic>($"api/videochats/{SessionId}/end", new { }, token);
                    return RedirectToPage("/VideoChat/Index");
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to end session.";
                }
            }

            return Page();
        }
    }
}