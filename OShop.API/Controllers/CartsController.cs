using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OShop.API.Data;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Carts;
using System.Security.Claims;
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
        public async Task<IActionResult> AddToCart([FromRoute]int productId,CancellationToken cancellationToken)
        {
            //var appUser =  _userManager.GetUserId(User);//this error when use token cuze this mybe get the id from cookies
            /*to get the id from tokens*/
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;//during this is can get any value from token 
            var result = await _cartService.AddToCart(appUser, productId, cancellationToken);
            return Ok();
        }
        [HttpGet("")]
        public async Task<IActionResult> getUserCartAsync()
        {
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var cartItems = await _cartService.getUserCartAsync(appUser);
            var cartResponse = cartItems.Select(c=>c.product).Adapt<IEnumerable<CartResponse>>();
            var totalPrice = cartItems.Sum(e => e.product.Price * e.Count);
            return Ok(new {cartResponse,totalPrice});
        }
    }
}
