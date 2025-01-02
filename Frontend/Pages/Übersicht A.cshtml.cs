using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class Übersicht_AModel : PageModel
    {
        private readonly RentalService rentalService;

        public Übersicht_AModel(RentalService rentalService)
        {
            this.rentalService = rentalService;
        }


        public List<Rental> rentals { get; private set; }


        public async Task <IActionResult> OnGet()
        {
            rentals = await rentalService.GetRentalsAsync();

            return Page();
        }
    }
}
