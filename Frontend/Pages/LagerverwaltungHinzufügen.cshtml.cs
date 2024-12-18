using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class LagerverwaltungHinzufügenModel : PageModel
{

    private readonly ProductService _productService;

    public LagerverwaltungHinzufügenModel(ProductService productService)
    {
        this._productService = productService;
    }


    // Produktinfos
    public class ProduktModel
    {
        public int ProduktID { get; set; }
        public string Produktname { get; set; }
        public string Produktbeschreibung { get; set; }
        public IFormFile? Produktbild { get; set; }
        public bool Produktstatus { get; set; }
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
            Produktbild = null,
            Produktmenge = 0,
            Produktstatus = true,
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

        var produktname = Request.Form["produktname"];
        var produktbezeichnung = Request.Form["produktbeschreibung"];
        var produktstatus = Request.Form["produktstatus"];


        //Schreibe alle parameter vom Form in den Debugger
        System.Diagnostics.Debug.WriteLine(produktname);
        System.Diagnostics.Debug.WriteLine(produktbezeichnung);
        System.Diagnostics.Debug.WriteLine(produktstatus);


        Product produkt = new Product()
        {
            productname = produktname,
            productdescription = produktbezeichnung,
            productactive = Convert.ToBoolean(produktstatus),
        };


        bool success = await _productService.addProductAsync(produkt);

        System.Diagnostics.Debug.WriteLine(success);

        return RedirectToPage("/Lagerverwaltung");
    }
}

