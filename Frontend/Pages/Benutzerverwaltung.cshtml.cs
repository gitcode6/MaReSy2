using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


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

    }
}
