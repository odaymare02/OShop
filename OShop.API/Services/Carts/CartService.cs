using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Cart> AddToCart(string userId,int productId,CancellationToken cancellationToken)
        {
            var existingCartItems = await _context.carts.FirstOrDefaultAsync(e => e.ApplicationUserId == userId && e.ProductId == productId);
            if(existingCartItems is not null)//the user have same product only increase the count
            {
                existingCartItems.Count += 1;
                await _context.SaveChangesAsync();
            }
            else
            {
                existingCartItems = new Cart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Count = 1
                };
                await _context.carts.AddAsync(existingCartItems, cancellationToken);
                await _context.SaveChangesAsync();
            }
            return existingCartItems;
        }
        public async Task<IEnumerable<Cart>> getUserCartAsync(string userId)
        {
            return await GetAsync(c=>c.ApplicationUserId==userId,includes: [c=>c.product]);
        }
        public async Task<bool> RemoveRangeAsync(List<Cart>items, CancellationToken cancellationToken = default)
        {
          
            _context.RemoveRange(items);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
