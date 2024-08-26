using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class BenutzerverwaltungBearbeitenModel : PageModel
{
    // Das Model für die Benutzerinformationen
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

    // Diese Methode wird aufgerufen, wenn die Seite geladen wird
    public IActionResult OnGet(int id)
    {
        // Hier solltest du den Benutzer anhand der ID aus der Datenbank laden
        // Beispielhaft wird hier ein statischer Benutzer geladen:
        Benutzer = new BenutzerModel
        {
            BenutzerID = id,
            Benutzername = "scheissalex1",
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

    // Diese Methode wird aufgerufen, wenn das Formular abgeschickt wird
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Hier solltest du die Änderungen in der Datenbank speichern
        // Beispielhaft wird hier kein tatsächlicher Datenbank-Update durchgeführt:

        // Zeige eine Bestätigung oder leite zur Benutzerverwaltungsseite zurück
        return RedirectToPage("/Benutzerverwaltung");
    }
}
