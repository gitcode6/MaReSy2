using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class BenutzerverwaltungHinzufügenModel : PageModel
{
    // Benutzerinformationen
    public class BenutzerModel
    {
        public int BenutzerID { get; set; }
        public string Benutzername { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Passwort { get; set; }
        public string Rolle { get; set; }
        public string Email { get; set; } 
    }

    [BindProperty]
    public BenutzerModel Benutzer { get; set; }

    private readonly UserService userService;

    public BenutzerverwaltungHinzufügenModel(UserService userService)
    {
        this.userService = userService;
    }



    public IActionResult OnGet(int id)
    {

        // Beispielbenutzer
        Benutzer = new BenutzerModel
        {
            BenutzerID = id,
            Benutzername = "",
            Vorname = "",
            Nachname = "",
            Passwort = "",
            Rolle = "User"
        };

        if (Benutzer == null)
        {
            return NotFound();
        }

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var username = Request.Form["benutzername"];
        var firstname = Request.Form["vorname"];
        var lastname = Request.Form["nachname"];
        var passwort = Request.Form["passwort"];
        var role = Request.Form["rolle"];
        var email = Request.Form["email"];

        User user = new User()
        {
            username = username,
            firstname = firstname,
            lastname = lastname,
            password = passwort,
            role = role,
            email = email
        };


        bool success = await userService.addUserAsync(user);
        if(success == true)
        { TempData["FehlerMeldungGrün"] = "Benutzer:in-Erstellung war erfolgreich!"; }
        else
        {
          TempData["FehlerMeldungRot"] = "Benutzer:in-Erstellung war nicht erfolgreich!";
        }

        System.Diagnostics.Debug.WriteLine(success);

        return RedirectToPage("/Benutzerverwaltung");
    }
}
