using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class Übersicht_UModel : PageModel
    {
        private readonly RentalService rentalService;

        public Übersicht_UModel(RentalService rentalService)
        {
            this.rentalService = rentalService;
        }

        public List<Rental> userRentals { get; set; }

        public async Task<IActionResult> OnGet()
        {
            userRentals = await rentalService.GetUserRentalsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            bool success = await rentalService.cancelRentalAsync(id);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Rental-Stornierung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Rental-Stornierung war nicht erfolgreich!";
            }
            return RedirectToPage();
        }
    }
}
