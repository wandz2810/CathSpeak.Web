using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Friends
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<AccountDetailDto> Friends { get; set; } = new();
        public List<PendingRequestDto> PendingRequests { get; set; } = new();

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                Friends = await _apiService.GetAsync<List<AccountDetailDto>>("api/friendship/friends", token) ?? new();
                PendingRequests = await _apiService.GetAsync<List<PendingRequestDto>>("api/friendship/requests", token) ?? new();
            }
        }

        public async Task<IActionResult> OnPostAcceptRequestAsync(int friendshipId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var responseData = new { Action = "accept" };
                await _apiService.PutAsync<dynamic>($"api/friendship/respond/{friendshipId}", responseData, token);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeclineRequestAsync(int friendshipId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var responseData = new { Action = "decline" };
                await _apiService.PutAsync<dynamic>($"api/friendship/respond/{friendshipId}", responseData, token);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveFriendAsync(int friendshipId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                await _apiService.DeleteAsync($"api/friendship/{friendshipId}", token);
            }

            return RedirectToPage();
        }
    }

    public class PendingRequestDto
    {
        public int FriendshipId { get; set; }
        public DateTime CreateDate { get; set; }
        public AccountDetailDto? Requester { get; set; }
    }
}