using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class ReservierungDetailModel : PageModel
    {
        private readonly RentalService rentalService;

        public ReservierungDetailModel(RentalService rentalService)
        {
            this.rentalService = rentalService;
        }


        public Rental rental { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            rental = await rentalService.GetRentalAsync(id);

            return Page();
        }
    }
}
