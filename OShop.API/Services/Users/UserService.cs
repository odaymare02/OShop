using Microsoft.AspNetCore.Identity;
using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.Users
{
    public class UserService:Service<ApplicationUser>,IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager< ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context,UserManager<ApplicationUser> userManager):base(context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> ChangeRole(string userId, string newRole)
        {
            var User = await _userManager.FindByIdAsync(userId);
            if(User is not null)
            {
                var oldRoles = await _userManager.GetRolesAsync(User);
                await _userManager.RemoveFromRolesAsync(User, oldRoles);
               var res= await _userManager.AddToRoleAsync(User, newRole);
                if (res.Succeeded) return true;
                return false;
            }
            return false;
        }
    }
}
