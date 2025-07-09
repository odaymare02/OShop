using Microsoft.AspNetCore.Mvc;
using OShop.API.Models;
using OShop.API.Services.IService;

namespace OShop.API.Services.Carts
{
    public interface ICartService:IService<Cart>
    {
        Task<Cart> AddToCart(string userId, int productId, CancellationToken cancellationToken);
        Task<IEnumerable<Cart>> getUserCartAsync(string userId);
        Task<bool> RemoveRangeAsync(List<Cart> items, CancellationToken cancellationToken = default);

    }
}
