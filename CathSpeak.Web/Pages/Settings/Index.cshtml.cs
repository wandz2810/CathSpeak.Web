using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CathSpeak.Web.Pages.Settings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public SettingsData Settings { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Load settings from session or database
            Settings = new SettingsData
            {
                EmailNotifications = true,
                FriendRequests = true,
                VideoCallInvitations = true,
                MessageSounds = true,
                AutoJoinVideoCall = false,
                ShowOnlineStatus = true,
                AllowMessagesFromStrangers = false
            };

            SuccessMessage = TempData["SuccessMessage"] as string;
        }

        public IActionResult OnPost()
        {
            // Save settings to session or database
            // For now, just show success message
            TempData["SuccessMessage"] = "Settings saved successfully!";
            return RedirectToPage();
        }
    }

    public class SettingsData
    {
        public bool EmailNotifications { get; set; }
        public bool FriendRequests { get; set; }
        public bool VideoCallInvitations { get; set; }
        public bool MessageSounds { get; set; }
        public bool AutoJoinVideoCall { get; set; }
        public bool ShowOnlineStatus { get; set; }
        public bool AllowMessagesFromStrangers { get; set; }
    }
}