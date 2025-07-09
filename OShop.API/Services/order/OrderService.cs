using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.order
{
    public class OrderService:Service<Order>,IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
