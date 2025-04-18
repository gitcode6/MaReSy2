using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;

public class LoginModel : PageModel
{
    private readonly UserService _userService;

    public LoginModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public User meinUser { get; set; }
    [BindProperty]
    public User meinLoginUser { get; set; }

    public async Task<IActionResult> OnGet()
    {
        await HttpContext.SignOutAsync();
        return Page();

        try
        {
            // Versuche, eine Verbindung aufzubauen (z.B. �ber einen Socket)
            var client = new TcpClient("/api", 80);
            await HttpContext.SignOutAsync();
            return Page();
        }
        catch (SocketException ex)
        {
            return RedirectToPage("/APIError");
        }
       
    }

	public async Task<IActionResult> OnPostAsync()
	{
        Debug.WriteLine(meinUser.username);
        Debug.WriteLine(meinUser.password);

        if (ModelState.IsValid)
        {
            meinLoginUser = await _userService.GetLoginAsync(meinUser);

            if (meinLoginUser != null && meinLoginUser.role == "Admin")
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, meinLoginUser.username),
                new Claim(ClaimTypes.Role, "Admin")
            };

                var identity = new ClaimsIdentity(claims, "AuthType");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);

                return RedirectToPage("/Dashboard");
            }
            else if (meinLoginUser!= null && meinLoginUser.role == "User")
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, meinLoginUser.username),
                new Claim(ClaimTypes.Role, "User2")
            };

                var identity = new ClaimsIdentity(claims, "AuthType");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);

                return RedirectToPage("/Dashboard");
            }
            else
            {
                if (meinUser.username == "")
                {
                    ModelState.AddModelError(string.Empty, "Benutzername leer");

                }
                if (meinUser.password == "")
                {
                    ModelState.AddModelError(string.Empty, "Passwort leer");
                }

                TempData["FehlerMeldungRot"] = "Login war nicht erfolgreich!";

                return RedirectToPage("/Login");
            }
        }
        return Page();
	}
}

