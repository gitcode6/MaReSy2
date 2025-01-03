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

        public async Task<IActionResult> OnPostChangeStatusAsync(int id, string direction)
        {
            var rental = await rentalService.GetRentalAsync(id);

            if(!RentalActions.Actions.TryGetValue(rental.status, out var actions))
            {
                return Page();

            }

            var actionCode = direction switch
            {
                "forward" => actions.NextAction,
                "backward" => actions.PreviousAction,
                _ => null
            };

            var update = new UpdateRentalDTO
            {
                action = actionCode.Value,
                rentalId = rental.rentalId,
            };


            var success = await rentalService.bearbeitenRentalAsync(update);

            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Set-Reservierung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Set-Reservierung war nicht erfolgreich!";
            }
            System.Diagnostics.Debug.WriteLine(success);


            rentals = await rentalService.GetRentalsAsync();

            return RedirectToPage("/Übersicht A");
        }

    }
}
