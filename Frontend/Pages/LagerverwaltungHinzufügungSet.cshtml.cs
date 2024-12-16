using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class LagerverwaltungHinzufügungSetModel : PageModel
    {
        private readonly ProductService _productService;
            
        public LagerverwaltungHinzufügungSetModel(ProductService productService) 
        { 
            _productService = productService;
        }

        public List<Product> products { get; set; }

        public async Task OnGetAsync()
        {
            products = await _productService.GetProductsAsync();
        }

        
    }
}
