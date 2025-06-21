using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CathSpeak.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Clear authentication cookies
                await HttpContext.SignOutAsync("CookieAuth");

                // Clear session data
                HttpContext.Session.Clear();

                // Log the logout action
                _logger.LogInformation($"User {User.Identity?.Name ?? "Unknown"} logged out at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

                // Set success message for login page
                TempData["SuccessMessage"] = "You have been successfully logged out. Thank you for using CathSpeak!";

                // Show logout page briefly before redirect
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");

                // Even if there's an error, still try to clear session and redirect
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "There was an issue during logout, but you have been signed out.";

                return RedirectToPage("/Auth/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Handle POST requests the same way as GET
            return await OnGetAsync();
        }
    }
}