using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class BenutzerverwaltungBearbeitenModel : PageModel
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
            Benutzername = "alexschmidt12",
            Vorname = "Ali",
            Nachname = "Schmiddi",
            Passwort = "lexi123",
            Rolle = "Admin"
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
