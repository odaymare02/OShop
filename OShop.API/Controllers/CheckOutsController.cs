using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.Models;
using OShop.API.Services.Carts;
using OShop.API.Services.order;
using OShop.API.Services.OrderItemServic;
using OShop.API.Utality.email;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutsController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly IOrderService orderService;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderItemService orderItemService;

        public CheckOutsController(ICartService cartService,IOrderService orderService,IEmailSender emailSender,UserManager<ApplicationUser> userManager,IOrderItemService orderItemService)
        {
            this.cartService = cartService;
            this.orderService = orderService;
            this.emailSender = emailSender;
            this.userManager = userManager;
            this.orderItemService = orderItemService;
        }
        [HttpGet("pay")]
        public async Task<IActionResult> pay([FromBody] PaymnetRequest request)
        {
            //any user need to pay first need to know his cart
            var appUser =  User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var carts = await cartService.GetAsync(e => e.ApplicationUserId == appUser, includes: [c=>c.product]);
            if(carts is not null)
            {
                Order order = new()
                {
                    ApplicationUserId = appUser,
                    OrderStatus = OrderStatus.Pending,
                    OrderDate = DateTime.Now,
                    TotalPrice = carts.Sum(e => e.product.Price * e.Count)
                };
                if (request.PaymentMethod == "Cash")
                {
                    order.paymentMethod = PaymentMethodType.Cash;
                    await orderService.AddAsync(order);
                    return RedirectToAction(nameof(Success), new { orderId = order.Id });//this the param was this action need it
                }
                else if(request.PaymentMethod=="Visa")
                {
                    await orderService.AddAsync(order);

                    order.paymentMethod = PaymentMethodType.Visa;
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Success/{order.Id}",
                        CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel"
                    };
                    /*to add each item in cart to lineitems*/
                    foreach (var item in carts)
                    {
                        options.LineItems.Add(
                              new SessionLineItemOptions
                              {
                                  PriceData = new SessionLineItemPriceDataOptions
                                  {
                                      Currency = "USD",
                                      ProductData = new SessionLineItemPriceDataProductDataOptions
                                      {
                                          Name = item.product.Name,
                                          Description = item.product.Description,
                                      },
                                      UnitAmount = (long)item.product.Price * 100,
                                  },
                                  Quantity = item.Count,
                              }
                            );
                    }
                    var service = new SessionService();
                    var session = service.Create(options);
                    order.SessionId = session.Id;
                   await orderService.CommitAsync();
                    return Ok(new { session.Url });
                }
                else
                {
                    return BadRequest(new { message = "Invalid payment method" });
                }
            }
            else
            {
                return NotFound();
            }
        }
        
        [HttpGet("Success/{orderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Success([FromRoute] int orderId)
        {
            var order = await orderService.GetOne(e => e.Id == orderId);
            var appuser = await userManager.FindByIdAsync(order.ApplicationUserId);
            //after user success paymnet will send an email the payment completed
            var subject = "";
            var body = "";
            var carts = await cartService.getUserCartAsync(appuser.Id);
            List<OrderItem> orderItems = [];
            foreach(var item in carts)
            {
                orderItems.Add(new()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    TotalPrice = item.product.Price * item.Count
                });
                item.product.Quantity -= item.Count;
            }
            await orderItemService.AddRangeAsync(orderItems);
            await cartService.RemoveRangeAsync(carts.ToList());
            if (order.paymentMethod == PaymentMethodType.Cash)
            {
                subject = " Order Recived-Cash Payment";
                body = $"<h1>Hello {appuser.UserName}</h1>" + $"<p> your order with {order.Id} has been place succesufuly</p>";
                await emailSender.SendEmailAsync(appuser.Email, subject, body);
            }
            else
            {
                order.OrderStatus = OrderStatus.Approved;
                var service = new SessionService();
                var session = service.Get(order.SessionId);//to return the session for this order exactly
                order.TransactionId=session.PaymentIntentId;
                await orderService.CommitAsync();
                subject = " Order Payment Success";
                body = $"<h1>Hello {appuser.UserName}</h1>" + $"<p> THX for shopping with o-shop</p>";
                await emailSender.SendEmailAsync(appuser.Email, subject, body);
            }
            return Ok(new { message = "success" });
        }
    }
}
