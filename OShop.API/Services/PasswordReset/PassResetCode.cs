using OShop.API.Data;
using OShop.API.Services.IService;

namespace OShop.API.Services.PasswordReset
{
    public class PassResetCode:Service<OShop.API.Models.PassResetCode>,IPassResetCode
    {
        ApplicationDbContext _context;
        public PassResetCode(ApplicationDbContext context) : base(context)
        {
            context = _context;
        }
    }
}
