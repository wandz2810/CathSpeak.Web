using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Rooms
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IApiService _apiService;

        [BindProperty]
        public RoomCreateDto RoomData { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public CreateModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (!string.IsNullOrEmpty(token))
                {
                    var createdRoom = await _apiService.PostAsync<RoomDto>("api/rooms", RoomData, token);

                    if (createdRoom != null)
                    {
                        TempData["SuccessMessage"] = "Room created successfully!";
                        return RedirectToPage("/Rooms/Index");
                    }
                }

                ErrorMessage = "Failed to create room. Please try again.";
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while creating the room.";
            }

            return Page();
        }
    }

    public class RoomCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}