using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaReSy2.Pages
{
    public class informationModel : PageModel
    {
        private readonly UserService userService;

        [BindProperty]
        public User angemeldeterUser { get; set; }

        public informationModel(UserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> OnGet()
        {
            angemeldeterUser = UserManager.GetUser();

            TempData["UserID"] = angemeldeterUser.userId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            angemeldeterUser.userId = Convert.ToInt32(TempData["UserID"]);
            bool success = await userService.passwordBearbeiten(angemeldeterUser);
            if (success == true)
            { TempData["FehlerMeldungGrün"] = "Passwort-Änderung war erfolgreich!"; }
            else
            {
                TempData["FehlerMeldungRot"] = "Passwort-Änderung war nicht erfolgreich!";
            }
            return RedirectToPage("/Information");
        }

    }
}
