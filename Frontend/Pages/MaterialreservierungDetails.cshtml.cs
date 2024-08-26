using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class MaterialreservierungDetailsModel : PageModel
    {
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public void OnGet(string id)
        {
            // Hier solltest du die Logik einfügen, um Materialdetails anhand der ID zu laden
            // Beispielwerte:
            MaterialName = "Name des Produktes";
            Description = "Beschreibung des Produktes.";
            Price = 0.00m;
        }
    }
}
