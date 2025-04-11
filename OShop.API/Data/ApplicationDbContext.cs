using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OShop.API.Models;

namespace OShop.API.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>//this handle inside all things about all tabel of identity 
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Brand> brands { get; set; }
    }
}
