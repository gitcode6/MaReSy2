using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace MaReSy2.Pages 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Benutzer> Benutzer { get; set; }
        public DbSet<Produkte> Produkt { get; set; }
    }
}
