using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public class LoginModel : PageModel
{
	[BindProperty]
	public string Benutzername { get; set; }

	[BindProperty]
	public string Passwort { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "gast"),
                new Claim(ClaimTypes.Role, "Gast")
            };

        var identity = new ClaimsIdentity(claims, "AuthType");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);

        return Page();
    }

	public async Task<IActionResult> OnPostAsync()
	{
		// Überprüfen der Anmeldeinformationen (dies ist nur ein Beispiel)
		if (Benutzername == "admin" && Passwort == "password")
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "admin"),
				new Claim(ClaimTypes.Role, "Admin")
			};

			var identity = new ClaimsIdentity(claims, "AuthType");
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(principal);

			return RedirectToPage("/Dashboard");
		}
        else if (Benutzername == "user" && Passwort == "password")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "User2")
            };

            var identity = new ClaimsIdentity(claims, "AuthType");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return RedirectToPage("/Dashboard");
        }
        else if (Benutzername == "gast" && Passwort == "password")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "gast"),
                new Claim(ClaimTypes.Role, "Gast")
            };

            var identity = new ClaimsIdentity(claims, "AuthType");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return RedirectToPage("/Login");
        }
        else
		{
			if(Benutzername == "")
			{
				ModelState.AddModelError(string.Empty, "Benutzername leer");
                
            }
			if (Passwort == "")
			{
				ModelState.AddModelError(string.Empty, "Passwort leer");
            }
		}

		


		return Page();
	}
}

