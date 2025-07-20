using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.ViewModels;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IApiService _apiService;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public RegisterViewModel RegisterData { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public RegisterModel(IApiService apiService, ILogger<RegisterModel> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public void OnGet()
        {
            ErrorMessage = TempData["ErrorMessage"] as string;
            SuccessMessage = TempData["SuccessMessage"] as string;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Use default avatar image instead of user input
                RegisterData.AvatarImageUrl = "/img/default-avatar.png";
                
                var registerRequest = new
                {
                    Username = RegisterData.Username,
                    Email = RegisterData.Email,
                    Password = RegisterData.Password,
                    AvatarImageUrl = RegisterData.AvatarImageUrl,
                    Address = RegisterData.Address ?? "string",
                    DateOfBirth = RegisterData.DateOfBirth,
                    Level = RegisterData.Level ?? "Beginner"
                };

                var response = await _apiService.PostAsync<object>("/api/auth/register", registerRequest);

                if (response is not null)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please login with your credentials.";
                    return RedirectToPage("/Auth/Login");
                }

                ErrorMessage = "Registration failed. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                ErrorMessage = "An error occurred during registration";
            }

            return Page();
        }
    }
}