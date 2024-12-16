using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class LagerverwaltungEPHinzuModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly SingleProductService _singleProductService;

        public LagerverwaltungEPHinzuModel(ProductService productService, SingleProductService singleProductService)
        {
            this._productService = productService;
            this._singleProductService = singleProductService;
        }

        public class SingleProduktModel
        {
            public int SingleProductID { get; set; }
            public string SingleProductName { get; set; }
            public string SingleProductSerialNumber { get; set; }
            public bool SingleProductStatus { get; set; }
            public int productId {  get; set; }
        }

        [BindProperty]
        public SingleProduktModel SingleProdukt { get; set; }
        public List<Product> products { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            products = await _productService.GetProductsAsync();

            SingleProdukt = new SingleProduktModel
            {
                SingleProductID = id,
                SingleProductName = "",
                SingleProductSerialNumber = "",
                productId = 21,
                SingleProductStatus = true,
            };

            if (SingleProdukt == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/LagerverwaltungEP");
            }

            var singleproductname = Request.Form["einzelproduktname"];
            var singleproductserialnumber = Request.Form["seriennummer"];
            var mainproduct = Request.Form["epkategorie"];
            var singleproductstatus = Request.Form["produktstatus"];
            


            //Schreibe alle parameter vom Form in den Debugger
            System.Diagnostics.Debug.WriteLine(singleproductname);
            System.Diagnostics.Debug.WriteLine(singleproductserialnumber);
            System.Diagnostics.Debug.WriteLine(singleproductstatus);
            System.Diagnostics.Debug.WriteLine(mainproduct);


            SingleProduktModel singleproduct = new SingleProduktModel()
            {
                SingleProductName = singleproductname,
                SingleProductSerialNumber = singleproductserialnumber,
                SingleProductStatus = Convert.ToBoolean(singleproductstatus),
                productId = Convert.ToInt32(mainproduct),
            };

            bool success = await _singleProductService.addSingleProductAsync(singleproduct);

            System.Diagnostics.Debug.WriteLine(success);

            return RedirectToPage("/LagerverwaltungEP");
        }
    }
}
