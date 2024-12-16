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
        private readonly SetService _setService;

        public List<Product> products { get; private set; }
        public List<Set> sets { get; private set; }


        public MaterialreservierungModel(ProductService productService, SetService setService)
        {
            _productService = productService;
            _setService = setService;
        }

        public async Task OnGetAsync()
        {
           products = await _productService.GetProductsAsync();
           sets = await _setService.GetSetsAsync();
        }
    }

}
