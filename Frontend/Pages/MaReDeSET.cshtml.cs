using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class MaReDeSETModel : PageModel
    {
        private readonly SetService setService;
        private readonly RentalService rentalService;
        public MaReDeSETModel(SetService setService, RentalService rentalService)
        {
            this.setService = setService;
            this.rentalService = rentalService;
        }

        public Set set { get; set; }

        private MakeRental rental { get; set; }
        public async Task<IActionResult> OnGet(int id)
        {
          set = await setService.GetSetAsync(id);
            TempData["SetId"] = set.setId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            rental = new MakeRental
            {
                setId = Convert.ToInt32(TempData["SetId"]),
                fromDate = DateOnly.FromDateTime(Convert.ToDateTime(Request.Form["startDatum"])),
                endDate = DateOnly.FromDateTime(Convert.ToDateTime(Request.Form["endDatum"])),
                rentalNote = Request.Form["rentalnote"]
            };


            bool success = await rentalService.makeRentalAsync(rental);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Set-Reservierung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Set-Reservierung war nicht erfolgreich!";
            }
            System.Diagnostics.Debug.WriteLine(success);

            return RedirectToPage("/Materialreservierung");
        }
    }
}
