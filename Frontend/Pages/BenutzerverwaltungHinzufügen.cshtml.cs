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
    }

    [BindProperty]
    public BenutzerModel Benutzer { get; set; }


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

        return RedirectToPage("/Benutzerverwaltung");
    }
}
