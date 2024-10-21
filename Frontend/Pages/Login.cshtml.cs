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

	public async Task<IActionResult> OnPostAsync()
	{
		// Überprüfen der Anmeldeinformationen (dies ist nur ein Beispiel)
		if (Benutzername == "admin" && Passwort == "password")
		{

			/*var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, Benutzername)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties
			{
				IsPersistent = true
			};

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
			*/

			return RedirectToPage("/Dashboard");
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

