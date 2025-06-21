using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Rooms
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<RoomDto> MyRooms { get; set; } = new();

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                MyRooms = await _apiService.GetAsync<List<RoomDto>>("api/rooms", token) ?? new();
            }
        }

        public async Task<IActionResult> OnPostDeleteRoomAsync(int roomId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                await _apiService.DeleteAsync($"api/rooms/{roomId}", token);
            }

            return RedirectToPage();
        }
    }
}