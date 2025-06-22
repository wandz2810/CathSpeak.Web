using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;
using System.Text.Json;

namespace CathSpeak.Web.Pages.VideoChat
{
    [Authorize]
    public class SessionModel : PageModel
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SessionModel> _logger;

        public VideoChatSessionDto? Session { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SessionId { get; set; }

        public SessionModel(IApiService apiService, ILogger<SessionModel> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Please log in to access video chat sessions.";
                return Page();
            }

            try
            {
                // Get session details
                Session = await _apiService.GetAsync<VideoChatSessionDto>($"api/videochats/{SessionId}", token);

                if (Session == null)
                {
                    ErrorMessage = "Session not found or you don't have access to it.";
                    return Page();
                }

                // Check if user is a participant
                var currentUsername = User.Identity?.Name;
                bool isParticipant = Session.Participants.Any(p =>
                    p.Username == currentUsername && p.Status == 1);

                if (!isParticipant)
                {
                    // Add participant if they're not already in the session
                    await _apiService.PostAsync<dynamic>(
                        $"api/videochats/{SessionId}/join",
                        new { },
                        token);

                    // Refresh session data
                    Session = await _apiService.GetAsync<VideoChatSessionDto>($"api/videochats/{SessionId}", token);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load video chat session {SessionId}", SessionId);
                ErrorMessage = "Failed to load session details. Please try refreshing the page.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLeaveAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                await _apiService.PostAsync<dynamic>($"api/videochats/{SessionId}/leave", new { }, token);

                // Log successful leave action
                _logger.LogInformation("User {Username} left video session {SessionId}",
                    User.Identity?.Name, SessionId);

                return RedirectToPage("/VideoChat/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {Username} attempted to leave session {SessionId}",
                    User.Identity?.Name, SessionId);

                ErrorMessage = "Failed to leave session. Please try again.";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEndAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                await _apiService.PostAsync<dynamic>($"api/videochats/{SessionId}/end", new { }, token);

                // Log successful end action
                _logger.LogInformation("User {Username} ended video session {SessionId}",
                    User.Identity?.Name, SessionId);

                return RedirectToPage("/VideoChat/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {Username} attempted to end session {SessionId}",
                    User.Identity?.Name, SessionId);

                ErrorMessage = "Failed to end session. Please check if you have permission to end this session.";
                await OnGetAsync();
                return Page();
            }
        }
    }
}