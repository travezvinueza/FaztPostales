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

        
    }
}