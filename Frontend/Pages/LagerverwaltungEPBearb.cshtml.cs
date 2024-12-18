using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MaReSy2.Pages
{
    public class LagerverwaltungEPBearbModel : PageModel
    {
        private readonly SingleProductService singleProductService;
        private readonly ProductService productService;

        [BindProperty]
        public SingleProduct meinproduct {  get; set; }

        public LagerverwaltungEPBearbModel(ProductService productService, SingleProductService singleProductService)
        {
            this.productService = productService;
            this.singleProductService = singleProductService;
        }

        public List<Product> products { get; private set; }

        public async Task<IActionResult> OnGet(int id)
        {
            meinproduct = await singleProductService.GetSingleProductAsync(id);
            products = await productService.GetProductsAsync();

            TempData["SingleProductID"] = meinproduct.singleProductId;

            return Page();
        }

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

            meinproduct.singleProductId = Convert.ToInt32(TempData["SingleProductID"]);

            CreateSingleProductModel meinproduct2 = new CreateSingleProductModel();

            // Nur Felder setzen, die im Formular geändert wurden
            if (!string.IsNullOrEmpty(meinproduct.singleProductName))
            {
                meinproduct2.singleProductName = meinproduct.singleProductName;
            }

            if (!string.IsNullOrEmpty(meinproduct.singleProductSerialNumber))
            {
                meinproduct2.singleProductSerialNumber = meinproduct.singleProductSerialNumber;
            }

            meinproduct2.singleProductActive = meinproduct.singleProductActive;

            // Wenn die Kategorie geändert wurde
            if (meinproduct.mainProduct != "0")
            {
                meinproduct2.productId = Convert.ToInt32(meinproduct.mainProduct);
            }

            await singleProductService.bearbeitenSingleProductAsync(meinproduct2, meinproduct.singleProductId);

            return RedirectToPage("/LagerverwaltungEP");
        }
    }
}
