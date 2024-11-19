using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MaReSy2.Pages
{
    public class BenutzerverwaltungModel : PageModel
    {
        private readonly UserService userService;

        public List<User> users { get; private set; }


        public BenutzerverwaltungModel(UserService userService)
        {
            this.userService = userService;
        }

        public async Task OnGetAsync()
        {
            users = await userService.GetUsersAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            bool success = await userService.deleteUserAsync(id);

            users = await userService.GetUsersAsync();

            return Page();

            //var isDeleted = await userService.deleteUserAsync(id);

            //if (!isDeleted)
            //{
            //    ModelState.AddModelError("", $"Benutzer mit der Id {id} konnte nicht gelöscht werden.");
            //    return Page();
            //}

            ////users = await userService.GetUsersAsync();
            //return Page();
        }

    }
}
