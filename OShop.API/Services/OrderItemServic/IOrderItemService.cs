using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.OrderItemServic
{
    public interface IOrderItemService:IService<OrderItem>
    {
        Task<List<OrderItem>> AddRangeAsync(List<OrderItem> entities, CancellationToken cancellation = default);

    }
}
