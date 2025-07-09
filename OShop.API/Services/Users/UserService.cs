using Microsoft.AspNetCore.Identity;
using OShop.API.Data;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.IService;
using System.Threading.Tasks;

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
        public async Task<LockUnlockResponse> LockUnlock(string userId)//this function to handle block and unnblock
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new LockUnlockResponse
                {
                   Success=true,
                   Message="User Not Found",
                   IsLocked=null
                };
            }
            //check if user has block or not and if time of block over 
            var isLockedNow = user.LockoutEnabled && user.LockoutEnd > DateTime.Now;
            if (isLockedNow)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);

                return new LockUnlockResponse
                {
                    Success = true,
                    Message = "User has been unblocked",
                    IsLocked = false
                };
                //if user blocked this will remove the block
            }
            else
            {
                //here if user click this end point and need to block user 
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddMinutes(3);
                await _userManager.UpdateAsync(user);
                return new LockUnlockResponse
                {
                    Success = true,
                    Message = "User has been blocked for 3 minutes",
                    IsLocked = true

                };
            }
            
        }
    }
    }
