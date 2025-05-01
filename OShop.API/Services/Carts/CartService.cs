using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.Carts
{
    public class CartService : Service<Cart>, ICartService
    {
        private readonly ApplicationDbContext _context;
       public CartService(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
