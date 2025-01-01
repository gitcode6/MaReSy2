using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MaReSy2.Pages
{
    public class LagerverwaltungModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly SetService _setService;

        public LagerverwaltungModel(ProductService productService, SetService setService)
        {
            _productService = productService;
            _setService = setService;
        }

        public List<Product> products { get; private set; }
        public List<Set> sets { get; private set; }

        public async Task OnGetAsync()
        {
            products = await _productService.GetProductsAsync();
            sets = await _setService.GetSetsAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            bool success = await _productService.deleteProductAsync(id); 
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Produkt-Löschung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Produkt-Löschung war nicht erfolgreich!";
            }
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteSET(int id)
        {
            bool success = await _setService.deleteSetAsync(id);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "SET-Löschung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "SET-Löschung war nicht erfolgreich!";
            }
            return RedirectToPage();
        }
    }
}
