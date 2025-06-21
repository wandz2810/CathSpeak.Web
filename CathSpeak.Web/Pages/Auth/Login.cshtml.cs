using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using CathSpeak.Web.Models.ViewModels;
using CathSpeak.Web.Services;

namespace CathSpeak.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IApiService _apiService;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginViewModel LoginData { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public LoginModel(IApiService apiService, ILogger<LoginModel> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public void OnGet()
        {
            ErrorMessage = TempData["ErrorMessage"] as string;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var loginRequest = new
                {
                    Username = LoginData.Username,
                    Password = LoginData.Password
                };

                var response = await _apiService.PostAsync<object>("/api/Auth/login", loginRequest);

                if (response != null)
                {
                    var responseElement = (JsonElement)response;
                    var token = responseElement.GetProperty("token").GetString();
                    var user = responseElement.GetProperty("user");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.GetProperty("accountId").GetInt32().ToString()),
                        new Claim(ClaimTypes.Name, user.GetProperty("username").GetString() ?? ""),
                        new Claim(ClaimTypes.Email, user.GetProperty("email").GetString() ?? ""),
                        new Claim("Token", token ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("CookieAuth", principal);

                    // Store token in session
                    HttpContext.Session.SetString("Token", token ?? "");

                    return RedirectToPage("/Dashboard/Index");
                }

                ErrorMessage = "Invalid username or password";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                ErrorMessage = "An error occurred during login";
            }

            return Page();
        }
    }
}