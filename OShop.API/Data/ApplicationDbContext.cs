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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Cart>().HasKey(e => new { e.ProductId, e.ApplicationUser });
            base.OnModelCreating(builder);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<PassResetCode> passResetCodes { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<ReviewImages> reviewImages { get; set; }

    }
}
