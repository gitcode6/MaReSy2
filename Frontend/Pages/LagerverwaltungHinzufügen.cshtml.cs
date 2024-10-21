using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class LagerverwaltungHinzufügenModel : PageModel
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


    public IActionResult OnGet(int id)
    {

        // Beispielprodukt
        Produkt = new ProduktModel
        {
            ProduktID = id,
            Produktname = "",
            Produktbeschreibung = "",
            Produktbild = "",
            Produktmenge = 0,
            Produktstatus = "Aktiv",
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

