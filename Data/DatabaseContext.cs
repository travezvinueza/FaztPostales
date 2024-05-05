using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mvc.Models.Entity;

namespace Mvc.Data
{
    public class DatabaseContext : IdentityDbContext<Usuario>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Envio> Envios { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                 .HasMany(u => u.Envios)
                 .WithOne(e => e.Usuario)
                 .OnDelete(DeleteBehavior.Cascade);

        }

    }
}