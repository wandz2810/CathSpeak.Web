using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CathSpeak.Web.Models.DTOs;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public List<AccountDetailDto> Users { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
        public AdminStats Stats { get; set; } = new();

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                Users = await _apiService.GetAsync<List<AccountDetailDto>>("api/account", token) ?? new();
                Roles = await _apiService.GetAsync<List<RoleDto>>("api/role", token) ?? new();

                Stats = new AdminStats
                {
                    TotalUsers = Users.Count,
                    ActiveUsers = Users.Count(u => u.Status == 1),
                    TotalRoles = Roles.Count
                };
            }
        }
    }

    public class AdminStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRoles { get; set; }
    }

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}