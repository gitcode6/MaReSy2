using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace MaReSy2.Pages
{
    public class LagerverwaltungHinzufügungSetModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly SetService _setService;

        public LagerverwaltungHinzufügungSetModel(SetService setService, ProductService productService)
        {
            _setService = setService;
            _productService = productService;
        }

        public class ProduktModel
        {
            public int productId { get; set; }
            public int productAmount { get; set; }

        }

        public List<Product> products { get; set; }

        public Set set { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            set = new Set
            {
                setId = id,
                setactive = true,
                setdescription = "",
                setname = "",
                products = null,
            };

            if (set == null)
            {
                return NotFound();
            }

            products = await _productService.GetProductsAsync();

            return Page();
        }


        public List<CreateSetProductAmount> products2 { get; set; }


        [BindProperty]
        public string AnzahlDefinierenDaten { get; set; } // Das JSON vom Frontend
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var setname = Request.Form["setname"];
            var setbeschreibung = Request.Form["setbeschreibung"];
            var setstatus = Request.Form["setstatus"];



            CreateSetModel newSet = new CreateSetModel()
            {
                setname = setname,
                setdescription = setbeschreibung,
                setactive = Convert.ToBoolean(setstatus),
                products = null
            };

            if (!string.IsNullOrEmpty(AnzahlDefinierenDaten))
            {
                // JSON-Daten in ein C#-Objekt deserialisieren
                var produktListe = JsonConvert.DeserializeObject<List<CreateSetProductAmount>>(AnzahlDefinierenDaten);

                if (produktListe.Count > 0) { newSet.products = produktListe; }
            }

            bool success = await _setService.addSetAsync(newSet);

            System.Diagnostics.Debug.WriteLine(success);

            return RedirectToPage("/Lagerverwaltung");
        }
    }
}
