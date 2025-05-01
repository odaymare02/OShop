using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.Users
{
    public interface IUserService:IService<ApplicationUser>
    {
        Task<bool>ChangeRole(string userId, string newRole);
    }
}
