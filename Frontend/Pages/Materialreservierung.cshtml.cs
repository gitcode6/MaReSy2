using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class MaterialreservierungModel : PageModel
    {
        private readonly ProductService _productService;

        public List<Product> products { get; private set; }


        public MaterialreservierungModel(ProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync()
        {
           products = await _productService.GetProductsAsync();

        }
    }

}
