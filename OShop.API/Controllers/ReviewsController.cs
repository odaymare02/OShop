using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Migrations;
using OShop.API.Models;
using OShop.API.Services.order;
using OShop.API.Services.OrderItemServic;
using OShop.API.Services.REviews;
using Stripe;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/products/{productId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly IOrderItemService orderItemService;

        public ReviewsController(IReviewService reviewService,IOrderItemService orderItemService)
        {
            this.reviewService = reviewService;
            this.orderItemService = orderItemService;
        }
        [HttpPost("")]
        public async Task<IActionResult> Create(int productId,[FromForm]ReviewRequest  request)
        {
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var hasOrder = (await orderItemService.GetAsync(e => e.ProductId == productId &&appUser==e.Order.ApplicationUserId)).Any();//first return all products with this id have orderd after that it will filter to exact order for this user who login and need to review and return true or false cuze i use any()
            if (hasOrder)
            {
                //only allow one review for the user to this product
                var hasReview =( await reviewService.GetAsync(e=>e.ProductId==productId&& e.ApplicationUserId == appUser)).Any();
                if (!hasReview)
                {
                    var review = request.Adapt<Models.Review>();
                    review.ApplicationUserId = appUser;
                    review.ProductId = productId;
                    review.ReviewDate = DateTime.Now;
                    await reviewService.AddAsync(review);
                    return Ok();
                }
                return BadRequest(new { message = "you can't add two review to the same product" });
            }
            return BadRequest(new { message="can't review this product unless you not order it" });
        }
        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> getAll([FromRoute] int productId)
        {

            var reviews = await reviewService.GetAsync(e => e.ProductId == productId, includes:[e=>e.ApplicationUser]);
           
            var response = reviews.Adapt<List<ReviewResponse>>();
           
           
            return Ok(response);
        }
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> delete([FromRoute]int reviewId)
        {
            var hasReview = await reviewService.GetOne(e=>e.Id== reviewId);
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (hasReview is null)
                return BadRequest(new { message = "Review not found" });
            if(appUser is  null)
                return Forbid();
            await reviewService.RemoveAsync(reviewId);
            return Ok();
        }
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> edit( int reviewId,  int productId,[FromBody]editReviewRequest request)
        {
            var appUser= User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = await reviewService.GetOne(r =>
            r.Id == reviewId &&
            r.ProductId == productId &&
            r.ApplicationUserId == appUser
            );
            if (review is null)
                return NotFound(new { message = "Review not found or access denied" });
            review.Commnet = request.Commnet;
            review.Rate = request.Rate;
            review.ReviewDate = DateTime.UtcNow;
            await reviewService.CommitAsync();
            return Ok(new { message = "Review updated successfully" });

        }
    }
}
