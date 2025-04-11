using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.Models;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpPost("register")]//inside identity have a default endpoint for register take care about that
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {
            //when i need to add to database i will add applicationUser cuz of that sill make adabt
            var applicationUser = registerRequest.Adapt<ApplicationUser>();
            if (string.IsNullOrWhiteSpace(registerRequest.UserName) && !string.IsNullOrWhiteSpace(registerRequest.Email))
            {
                applicationUser.UserName = applicationUser.Email.Split('@')[0];
            }
            //we have usermannager class contain creatAsync to make lot of operation to make the register we will make object form it and then can use it
            //UserManager<ApplicationUser> userManager = new UserManager();//this have big problem cuz this object need 10 paramter if i need to create object manula but to solve this i make dependency injection
            var result= await userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(applicationUser, false);
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
             var applicationUser=await userManager.FindByEmailAsync(loginRequest.Email);
            if (applicationUser != null)
            {
                var result=await userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);
                if (result)
                {
                    await signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);
                    return NoContent();
                }

            }
            return BadRequest(new { message = "invalid email or password"});
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }
        [Authorize]

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
           var applicationUser= await userManager.GetUserAsync(User);//to get information of this user inside cookies
            if (applicationUser != null)
            {
               var result= await userManager.ChangePasswordAsync(applicationUser, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                   return BadRequest(result.Errors);
                }
            }
            return BadRequest(new { message = "Invalid Data" });
        }

    }
}
