
using System.Diagnostics;
using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class LagerverwaltungEPHinzuModel : PageModel
    {
        private readonly ProductService productService;
        private readonly SingleProductService singleProductService;
        public LagerverwaltungEPHinzuModel(ProductService productService, SingleProductService singleProductService)
        {
            this.productService = productService;
            this.singleProductService = singleProductService;
        }

        [BindProperty]
        public CreateSingleProductModel singleproduct { get; set; }

        public List<Product> products { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            products = await productService.GetProductsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            products = await productService.GetProductsAsync();
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

            CreateSingleProductModel productModel = new CreateSingleProductModel()
            {
                singleProductName = singleproduct.singleProductName,
                singleProductSerialNumber = singleproduct.singleProductSerialNumber,
                singleProductActive = singleproduct.singleProductActive,
                productId = singleproduct.productId,
            };

            var result = singleProductService.addSingleProductAsync(productModel);
            Debug.WriteLine(result);

            return RedirectToPage("/LagerverwaltungEP");
        }
    }
}
