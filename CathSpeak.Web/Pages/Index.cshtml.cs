using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CathSpeak.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // If user is authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Dashboard/Index");
            }

            // Otherwise show landing page
            return Page();
        }
    }
}