using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Users;
using OShop.API.Utality;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =$"{StaticData.SuperAdmin}")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;


        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {

            var users = await userService.GetAsync();
           return Ok(users.Adapt<IEnumerable<UserDto>>()); 
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute]string userId)
        {
             var user=await userService.GetOne(u => u.Id == userId);
            return Ok(user.Adapt<UserDto>());
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> ChangeRole([FromRoute]string userId,[FromQuery]string newRole)
        {
            var res =await userService.ChangeRole(userId, newRole);
            return Ok(res);
        }
    }
}
