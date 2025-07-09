using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.OrderItemServic
{
    public class OrderItemService:Service<OrderItem>,IOrderItemService
    {
        private readonly ApplicationDbContext _context;

        public OrderItemService(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<OrderItem>> AddRangeAsync(List<OrderItem> entities, CancellationToken cancellation = default)
        {
            await _context.AddRangeAsync(entities, cancellation);
            await _context.SaveChangesAsync(cancellation);
            return entities;
        }
    }
}
