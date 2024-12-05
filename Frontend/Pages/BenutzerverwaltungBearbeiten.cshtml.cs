using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;

public class BenutzerverwaltungBearbeitenModel : PageModel
{
    private readonly UserService userService;

    [BindProperty]
    public User meinUser { get; set; }
    // [TempData]
    // public int UserID { get; set; }

    public BenutzerverwaltungBearbeitenModel(UserService userService)
    {
        this.userService = userService;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        meinUser = await userService.GetUserAsync(id);

        TempData["UserID"] = meinUser.userId;

        return Page();
    }

    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        meinUser.userId = Convert.ToInt32(TempData["UserID"]);
        await userService.bearbeitenUserAsync(meinUser);

        return RedirectToPage("/Benutzerverwaltung");
    }
}
