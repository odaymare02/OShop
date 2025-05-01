using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.Carts;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToCart([FromRoute]int productId,[FromQuery]int count)
        {
            var appUser =  _userManager.GetUserId(User);
            var cart = new Cart
            {
                ProductId = productId,
                Count = count,
                ApplicationUserId = appUser
            };
            await _cartService.AddAsync(cart);
            return Ok(cart);
        }
    }
}
