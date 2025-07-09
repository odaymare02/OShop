using Mapster;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using OShop.API.Utality;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OShop.API.Services.PasswordReset;
using OShop.API.Utality.email;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPassResetCode passResetService;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IEmailSender emailSender,RoleManager<IdentityRole> roleManager,IPassResetCode passResetService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
            this.passResetService = passResetService;
        }
        [HttpPost("register")]//inside identity have a default endpoint for register take care about that
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {
            //when i need to add to database i will add applicationUser cuz of that sill make adabt
            var applicationUser = registerRequest.Adapt<ApplicationUser>();
            applicationUser.UserName=$"{ registerRequest.FirstName}{CapitalizeFirstLetter(registerRequest.LastName)}";
           

            //we have usermannager class contain creatAsync to make lot of operation to make the register we will make object form it and then can use it
            //UserManager<ApplicationUser> userManager = new UserManager();//this have big problem cuz this object need 10 paramter if i need to create object manula but to solve this i make dependency injection

            var result= await userManager.CreateAsync(applicationUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                var duplicatedUserName = result.Errors.FirstOrDefault(e => e.Code == "DuplicateUserName");
                if (duplicatedUserName != null && !string.IsNullOrWhiteSpace(applicationUser.Email))
                {
                    applicationUser.UserName = applicationUser.Email.Split('@')[0];
                    result = await userManager.CreateAsync(applicationUser, registerRequest.Password);
                }
            }
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(applicationUser, StaticData.Customre);
               var token= await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);//this is unique token valid for one time only
                var emailConfirmURL = Url.Action(nameof(ConfirmEmail),"Account",/*this is the param of action*/ new {token,userId=applicationUser.Id},
                    protocol:Request.Scheme,
                    host:Request.Host.Value);//this to get the domain dynamic
                var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to O_Shop</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding: 10px 0;
            border-bottom: 2px solid #f0f0f0;
        }}
        .header img {{
            max-width: 150px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content h1 {{
            font-size: 24px;
            color: #333333;
        }}
        .content p {{
            font-size: 16px;
            color: #666666;
        }}
        .footer {{
            text-align: center;
            padding: 10px 0;
            font-size: 14px;
            color: #999999;
            border-top: 2px solid #f0f0f0;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #4CAF50;
            border-radius: 4px;
            text-decoration: none;
        }}
        @media (max-width: 600px) {{
            .email-container {{
                width: 100%;
                padding: 10px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <img src='https://yourwebsite.com/logo.png' alt='Website Logo'>
        </div>
        <div class='content'>
            <h1>Welcome to O_Shop, {applicationUser.UserName}!</h1>
            <p>Thank you for registering with us. We are excited to have you on board. Below are your registration details:</p>
            <p><strong>Username:</strong> {applicationUser.UserName}</p>
            <p><strong>Email:</strong> {applicationUser.Email}</p>
            <p>Click the button below to confirm your email:</p>
            <p><a href='{emailConfirmURL}' class='button'>click Here</a></p>
        </div>
        <div class='footer'>
            <p>If you did not register for an account, please ignore this email.</p>
            <p>&copy; 2025 O_Shop. All Rights Reserved.</p>
        </div>
    </div>
</body>
</html>";
                await emailSender.SendEmailAsync(applicationUser.Email, "welcome dear", emailBody);
                //await signInManager.SignInAsync(applicationUser, false);//to add cookies
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
        private string CapitalizeFirstLetter(string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();

        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok(new { message = "email confirmed" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return NotFound();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if(!Regex.IsMatch(loginRequest.Email, @"^s\d{8}@stu\.najah\.edu$"))
            {
                return BadRequest(new { message = "Email must be a valid university email (s########@stu.najah.edu)" });
            }
            var applicationUser=await userManager.FindByEmailAsync(loginRequest.Email);
            if (applicationUser != null)
            {
                var result = await signInManager.PasswordSignInAsync(applicationUser, loginRequest.Password,loginRequest.RememberMe,false);//this check pass and confirm email and block if try over 5 times
                List<Claim> claimsp = new();
                claimsp.Add(new(ClaimTypes.Name, applicationUser.UserName));
                claimsp.Add(new(ClaimTypes.NameIdentifier, applicationUser.Id));
                var userRoles = await userManager.GetRolesAsync(applicationUser);
                if (userRoles.Count > 0)
                {
                    foreach(var item in userRoles)
                    {
                        claimsp.Add(new(ClaimTypes.Role, item));
                    }
                }
                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);
                    SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes("lYlc929meK4cIhLGJfzE0zDWr65UrEi0"));//this is signtuare key acoording this know you only can use this token
                    SigningCredentials signing = new(securityKey, SecurityAlgorithms.HmacSha256);//the type of algo 
                   var jwtToken= new JwtSecurityToken(
                        claims:claimsp,//what the payload of this token 
                        expires:DateTime.Now.AddMinutes(30),//the life time of this token
                        signingCredentials:signing
                        );
                    string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    return Ok(new {token});
                }
                else
                {
                    if (result.IsLockedOut) return BadRequest(new { message = "Your Account Is Locked" });
                    else if (result.IsNotAllowed) return BadRequest(new { message = "Your Email Not Confirmed,Please confirm it" });
                }

            } 
            return BadRequest(new { message = "invalid email or password"});
        }
        /*
        [HttpGet("LoginGoogle")]
        public IActionResult LoginGoogle()
        {
            // Explicitly specify the route template
            var redirectUrl = Url.Action(
                action: "GoogleResponse",
                controller: "Account",
                values: null,
                protocol: Request.Scheme
            );

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var firstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            if (email == null)
            {
                return BadRequest(new { message = "Google login failed: no email" });
            }
            var use = await userManager.FindByEmailAsync(email);
            if (use == null)
            {
                use = new ApplicationUser
                {
                    Email = email,
                    UserName = email.Split('@')[0],
                    EmailConfirmed = true,
                    FirstName = firstName ?? "Google",
                    LastName = lastName ?? "User",
                    Gender = ApplicationUserGender.Male, // Or Female, or make nullable if optional
                    BirthOfDate = DateTime.UtcNow.AddYears(-20), // Dummy DOBr
                };
                var identityResult = await userManager.CreateAsync(use);
                if (!identityResult.Succeeded)
                    return BadRequest(identityResult.Errors);
            }
            await signInManager.SignInAsync(use, isPersistent: false);
            return Ok(new { message = "Login successful", use.Email, use.FirstName, use.LastName });
        }
        */
        [HttpGet("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpPost("ChangePassword")]
        [Authorize]
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
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetRequest request)
        {
            var appUser = await userManager.FindByEmailAsync(request.Email);
            if(appUser is not null)
            {
                var code = new Random().Next(1000, 9999).ToString();
                await passResetService.AddAsync(new()
                {
                    ApplicationUserId = appUser.Id,
                    code = code,
                    ExpirationCode = DateTime.Now.AddMinutes(30)
                });
                var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to O_Shop</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding: 10px 0;
            border-bottom: 2px solid #f0f0f0;
        }}
        .header img {{
            max-width: 150px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content h1 {{
            font-size: 24px;
            color: #333333;
        }}
        .content p {{
            font-size: 16px;
            color: #666666;
        }}
        .footer {{
            text-align: center;
            padding: 10px 0;
            font-size: 14px;
            color: #999999;
            border-top: 2px solid #f0f0f0;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #4CAF50;
            border-radius: 4px;
            text-decoration: none;
        }}
        @media (max-width: 600px) {{
            .email-container {{
                width: 100%;
                padding: 10px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <img src='https://yourwebsite.com/logo.png' alt='Website Logo'>
        </div>
        <div class='content'>
            <h1>Welcome to O_Shop, {appUser.UserName}!</h1>
            <p>Thank you for registering with us. We are excited to have you on board. Below are your registration details:</p>
            <p><strong>Username:</strong> {appUser.UserName}</p>
            <p><strong>Email:</strong> {appUser.Email}</p>
            <p>your confirm code is{code}</p>
        </div>
        <div class='footer'>
            <p>If you did not register for an account, please ignore this email.</p>
            <p>&copy; 2025 O_Shop. All Rights Reserved.</p>
        </div>
    </div>
</body>
</html>";
                await emailSender.SendEmailAsync(appUser.Email, "forger Passwprd", emailBody);

                return Ok(new {mesage="Reset code sent"});
            }
            else
            {
                return BadRequest(new { message="user not found" });
            }
        }
        [HttpPatch("SendCode")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            var appUser = await userManager.FindByEmailAsync(request.Email);
            if(appUser is not null)
            {
               var resetCode= (await passResetService.GetAsync(e => e.ApplicationUserId == appUser.Id)).OrderByDescending(e=>e.ExpirationCode).FirstOrDefault();//to get the latest code
                if(resetCode is not null && resetCode.code == request.Code && resetCode.ExpirationCode > DateTime.Now)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
                    var result=await userManager.ResetPasswordAsync(appUser, token, request.Password);
                    if (result.Succeeded)
                    {
                        var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to O_Shop</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding: 10px 0;
            border-bottom: 2px solid #f0f0f0;
        }}
        .header img {{
            max-width: 150px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content h1 {{
            font-size: 24px;
            color: #333333;
        }}
        .content p {{
            font-size: 16px;
            color: #666666;
        }}
        .footer {{
            text-align: center;
            padding: 10px 0;
            font-size: 14px;
            color: #999999;
            border-top: 2px solid #f0f0f0;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #4CAF50;
            border-radius: 4px;
            text-decoration: none;
        }}
        @media (max-width: 600px) {{
            .email-container {{
                width: 100%;
                padding: 10px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <img src='https://yourwebsite.com/logo.png' alt='Website Logo'>
        </div>
        <div class='content'>
            <h1>Welcome to O_Shop, {appUser.UserName}!</h1>
            <p>Thank you for registering with us. We are excited to have you on board. Below are your registration details:</p>
            <p><strong>Username:</strong> {appUser.UserName}</p>
            <p><strong>Email:</strong> {appUser.Email}</p>
            <p>Success update password</p>
        </div>
        <div class='footer'>
            <p>If you did not register for an account, please ignore this email.</p>
            <p>&copy; 2025 O_Shop. All Rights Reserved.</p>
        </div>
    </div>
</body>
</html>";
                        await emailSender.SendEmailAsync(appUser.Email, "Change Password", emailBody);
                        return Ok(new { mesage = "password has been changed succesfully" });
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                else
                {
                    return BadRequest(new { message = "invalid code" });
                }
            }
            return BadRequest(new { message = "userNotFound" });
        }

    }
}
