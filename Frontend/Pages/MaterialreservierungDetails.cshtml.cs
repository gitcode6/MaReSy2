using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class MaterialreservierungDetailsModel : PageModel
    {
        private readonly ProductService productService;
        private readonly RentalService rentalService;
        public MaterialreservierungDetailsModel(ProductService productService, RentalService rentalService)
        {
            this.productService = productService;
            this.rentalService = rentalService;
        }



        private MakeRental rental { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        //public File ImageUrl { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            // Hier solltest du die Logik einfügen, um Materialdetails anhand der ID zu laden
            // Beispielwerte:

            var result = await productService.GetProductAsync(id);

            TempData["ProductId"] = result.productId;


            MaterialName = result?.productname;
            Description = result?.productdescription;
            //ImageUrl = getImageUrl(result.productId);





            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {


            rental = new MakeRental
            {
                productId = Convert.ToInt32(TempData["ProductId"]),
                productAmount = Convert.ToInt32(Request.Form["anzahl"]),
                fromDate = DateOnly.FromDateTime(Convert.ToDateTime(Request.Form["startDatum"])),
                endDate = DateOnly.FromDateTime(Convert.ToDateTime(Request.Form["endDatum"])),
                rentalNote = Request.Form["rentalnote"]
            };


            bool success = await rentalService.makeRentalAsync(rental);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Produkt-Reservierung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Produkt-Reservierung war nicht erfolgreich!";
            }
            System.Diagnostics.Debug.WriteLine(success);

            return RedirectToPage("/Materialreservierung");
        }

        //public async Task<IActionResult> getImageUrl(int productId)
        //{
        //    var imageBytes = await productService.GetProductImageAsync(productId);

        //    if (imageBytes != null)
        //    {
        //        return File(imageBytes, "image/jpg");

        //    }

        //    var standardImagePath = Path.Combine("wwwroot", "images", "Übungsmedikament1.png");
        //    var defaultImageBytes = await System.IO.File.ReadAllBytesAsync(standardImagePath);
        //    return File(defaultImageBytes, "image/png");
        //}
    }
}
