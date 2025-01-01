using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Benutzer abmelden
            await HttpContext.SignOutAsync();

            // Weiterleitung zur Login-Seite
            return RedirectToPage("/Login");
        }
    }
}
