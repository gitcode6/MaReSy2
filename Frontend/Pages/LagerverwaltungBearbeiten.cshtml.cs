using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class LagerverwaltungBearbeitenModel : PageModel
{
    // Produktinfos
    public class ProduktModel
    {
        public int ProduktID { get; set; }
        public string Produktname { get; set; }
        public string Produktbeschreibung { get; set; }
        public string Produktbild { get; set; }
        public string Produktstatus { get; set; }
        public int Produktmenge { get; set; }
    }

    [BindProperty]
    public ProduktModel Produkt { get; set; }


    public IActionResult OnGet(int id, string produktname, string produktbeschreibung, string produktbild, int menge, string status)
    {

        // Beispielprodukt
        Produkt = new ProduktModel
        {
            ProduktID = id,
            Produktname = produktname,
            Produktbeschreibung = produktbeschreibung,
            Produktbild = produktbild,
            Produktmenge = menge,
            Produktstatus = status,
        };

        if (Produkt == null)
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

        return RedirectToPage("/Lagerverwaltung");
    }
}
