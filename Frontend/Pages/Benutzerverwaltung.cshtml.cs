using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MaReSy2.Pages
{
    public class BenutzerverwaltungModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BenutzerverwaltungModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Benutzer> BenutzerListe { get; set; }

        public async Task OnGetAsync()
        {
            BenutzerListe = await _context.Benutzer.ToListAsync();
        }
    }
}