using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Friends
{
    [Authorize]
    public class SearchModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<AccountDetailDto> SearchResults { get; set; } = new();
        public List<AccountDetailDto> AllUsers { get; set; } = new();

        [BindProperty]
        public string SearchTerm { get; set; } = string.Empty;

        public string? Message { get; set; }

        public SearchModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                // Get all users for initial display
                AllUsers = await _apiService.GetAsync<List<AccountDetailDto>>("api/account", token) ?? new();
                SearchResults = AllUsers.Take(20).ToList(); // Show first 20 users initially
            }
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                AllUsers = await _apiService.GetAsync<List<AccountDetailDto>>("api/account", token) ?? new();

                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    SearchResults = AllUsers.Where(u =>
                        u.Username?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                        u.Email?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                        u.Level?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
                }
                else
                {
                    SearchResults = AllUsers.Take(20).ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendRequestAsync(int userId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var requestData = new { AddresseeId = userId };
                    await _apiService.PostAsync<dynamic>("api/friendship/request", requestData, token);
                    Message = "Friend request sent successfully!";
                }
                catch
                {
                    Message = "Failed to send friend request. They might already be your friend.";
                }
            }

            return await OnPostSearchAsync();
        }
    }
}