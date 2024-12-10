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

        public LagerverwaltungModel(ProductService productService)
        {
            _productService = productService;
        }

        public List<Product> products { get; private set; }
        public async Task OnGetAsync()
        {
            products = await _productService.GetProductsAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            bool success = await _productService.deleteProductAsync(id);
            return RedirectToPage();
        }
    }
}
