using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class LagerverwaltungBearbeitenModel : PageModel
{
    private readonly ProductService productService;

    [BindProperty]
    public Product meinProduct { get; set; }

    public LagerverwaltungBearbeitenModel(ProductService productService)
    {
        this.productService = productService;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        meinProduct = await productService.GetProductAsync(id);

        TempData["ProductID"] = meinProduct.productId;

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        meinProduct.productId = Convert.ToInt32(TempData["ProductID"]);
        await productService.bearbeitenProductAsync(meinProduct);

        return RedirectToPage("/Lagerverwaltung");
    }
}
