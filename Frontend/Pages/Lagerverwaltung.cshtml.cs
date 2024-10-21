using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MaReSy2.Pages
{
    public class LagerverwaltungModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LagerverwaltungModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IList<Produkte> ProduktListe { get; set; }

        public async Task OnGetAsync()
        {
            ProduktListe = await _context.Produkt.ToListAsync();
        }
    }
}
