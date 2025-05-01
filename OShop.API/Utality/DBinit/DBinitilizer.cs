using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OShop.API.Data;
using OShop.API.Models;

namespace OShop.API.Utality.DBinit
{
    public class DBinitilizer : IDBinitilizer
    {
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public DBinitilizer(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task initilizer()
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any())//if have any migration false convert to true
                    context.Database.Migrate();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (roleManager.Roles is not null)
            {
                await roleManager.CreateAsync(new(StaticData.SuperAdmin));
                await roleManager.CreateAsync(new(StaticData.Admin));
                await roleManager.CreateAsync(new(StaticData.Customre));
                await roleManager.CreateAsync(new(StaticData.Company));
                await userManager.CreateAsync(new()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "Super_Admin",
                    Gender = ApplicationUserGender.Male,
                    BirthOfDate = new DateTime(1995, 4, 3),
                    Email = "SuperAdmin@oshop.com"

                }, "SuperAdmin@1");//creat superAdmin direct when upload the project or run
                var user = await userManager.FindByEmailAsync("SuperAdmin@oshop.com");
                await userManager.AddToRoleAsync(user, StaticData.SuperAdmin);
            }
           
        }
    }
}
