using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Rooms
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IApiService _apiService;

        [BindProperty(SupportsGet = true)]
        public int RoomId { get; set; }

        [BindProperty]
        public RoomEditDto RoomData { get; set; } = new();

        public RoomDto? Room { get; set; }
        public string? ErrorMessage { get; set; }

        public EditModel(IApiService apiService)
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
                    Room = await _apiService.GetAsync<RoomDto>($"api/rooms/{RoomId}", token);

                    if (Room != null)
                    {
                        RoomData = new RoomEditDto
                        {
                            Name = Room.Name,
                            Description = Room.Description
                        };
                        return Page();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Failed to load room details.";
                }
            }

            return RedirectToPage("/Rooms/Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (!string.IsNullOrEmpty(token))
                {
                    var updateData = new
                    {
                        Name = RoomData.Name,
                        Description = RoomData.Description
                    };

                    await _apiService.PutAsync<dynamic>($"api/rooms/{RoomId}", updateData, token);

                    TempData["SuccessMessage"] = "Room updated successfully!";
                    return RedirectToPage("/Rooms/Index");
                }

                ErrorMessage = "Failed to update room. Please try again.";
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while updating the room.";
            }

            await OnGetAsync();
            return Page();
        }
    }

    public class RoomEditDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}