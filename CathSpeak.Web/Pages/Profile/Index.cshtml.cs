using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;
using System.Security.Claims;

namespace CathSpeak.Web.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public AccountDetailDto? Account { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public ProfileUpdateDto ProfileData { get; set; } = new();

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            await LoadProfile();
            ErrorMessage = TempData["ErrorMessage"] as string;
            SuccessMessage = TempData["SuccessMessage"] as string;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadProfile();
                return Page();
            }

            try
            {
                var token = HttpContext.Session.GetString("Token");
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userId))
                {
                    var updateData = new
                    {
                        Username = ProfileData.Username,
                        Email = ProfileData.Email,
                        AvatarImageUrl = ProfileData.AvatarImageUrl,
                        Address = ProfileData.Address,
                        DateOfBirth = ProfileData.DateOfBirth,
                        Level = ProfileData.Level
                    };

                    await _apiService.PutAsync<dynamic>($"api/account/{userId}", updateData, token);

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToPage();
                }

                ErrorMessage = "Failed to update profile. Please try again.";
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while updating your profile.";
            }

            await LoadProfile();
            return Page();
        }

        private async Task LoadProfile()
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userId))
            {
                Account = await _apiService.GetAsync<AccountDetailDto>($"api/account/{userId}", token);

                if (Account != null)
                {
                    ProfileData = new ProfileUpdateDto
                    {
                        Username = Account.Username,
                        Email = Account.Email,
                        AvatarImageUrl = Account.AvatarImageUrl,
                        Address = Account.Address,
                        DateOfBirth = Account.DateOfBirth,
                        Level = Account.Level
                    };
                }
            }
        }
    }

    public class ProfileUpdateDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? AvatarImageUrl { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Level { get; set; }
    }
}