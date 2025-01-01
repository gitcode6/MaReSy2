using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MaReSy2.Pages
{
    public class LagerverwaltungBearbeitenSETModel : PageModel
    {
        private readonly SetService setService;
        private readonly ProductService productService;

        [BindProperty]
        public Set meinSet {  get; set; }

        public LagerverwaltungBearbeitenSETModel(SetService setService, ProductService productService)
        {
            this.setService = setService;
            this.productService = productService;
        }

        public List<Product> products { get; private set; }

        public async Task<IActionResult> OnGet(int id)
        {
            meinSet = await setService.GetSetAsync(id);

            if (meinSet == null)
            {
                return NotFound();
            }

            products = await productService.GetProductsAsync();

            TempData["SetID"] = meinSet.setId;

            return Page();
        }

        public List<CreateSetProductAmount> products2 { get; set; }

        [BindProperty]
        public string AnzahlDefinierenDaten { get; set; } // Das JSON vom Frontend
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;

                    foreach (var error in errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exceptionMessage = error.Exception?.Message;

                        Debug.WriteLine($"Field: {key}, Error: {errorMessage}, Exception: {exceptionMessage}");
                    }
                }
                return Page();
            }
            products = await productService.GetProductsAsync();

            var setname = Request.Form["setname"];
            var setbeschreibung = Request.Form["setbeschreibung"];
            var setstatus = Request.Form["setstatus"];

            CreateSetModel newSet = new CreateSetModel()
            {
                setname = setname,
                setdescription = setbeschreibung,
                setactive = Convert.ToBoolean(setstatus),
                setProductAssignDTOs = null,
            };

            if (!string.IsNullOrEmpty(AnzahlDefinierenDaten))
            {
                // JSON-Daten in ein C#-Objekt deserialisieren
                var produktListe = JsonConvert.DeserializeObject<List<CreateSetProductAmount>>(AnzahlDefinierenDaten);

                if (produktListe.Count != null && produktListe.Any()) { newSet.setProductAssignDTOs = produktListe; }

            }

            meinSet.setId = Convert.ToInt32(TempData["SetID"]);
            bool success = await setService.bearbeitenSETAsync(newSet, meinSet.setId);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "SET-Bearbeitung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "SET-Bearbeitung war nicht erfolgreich!";
            }
            return RedirectToPage("/Lagerverwaltung");
        }
    }
}
