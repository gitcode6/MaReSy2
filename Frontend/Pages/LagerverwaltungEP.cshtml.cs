using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    

    public class LagerverwaltungEPModel : PageModel
    {
        //???
        private readonly SingleProductService _singleproductService;

        public LagerverwaltungEPModel(SingleProductService singleProductService)
        {
            _singleproductService = singleProductService;
        }

        public List<SingleProduct> singleProducts { get; private set; }
        public async Task OnGetAsync()
        {
            singleProducts = await _singleproductService.GetSingleProductsAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            bool success = await _singleproductService.deleteSingleProductAsync(id);
            if (success == true)
            { TempData["FehlerMeldungGr�n"] = "Einzelprodukt-L�schung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Einzelprodukt-L�schung war nicht erfolgreich!";
            }
            return RedirectToPage();
        }
    }
}
