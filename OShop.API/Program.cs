
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OShop.API.Data;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Brands;
using OShop.API.Services.Carts;
using OShop.API.Services.Categories;
using OShop.API.Services.order;
using OShop.API.Services.OrderItemServic;
using OShop.API.Services.PasswordReset;
using OShop.API.Services.Products;
using OShop.API.Services.REviews;
using OShop.API.Services.Users;

using OShop.API.Utality;
using OShop.API.Utality.DBinit;
using OShop.API.Utality.email;
using Scalar.AspNetCore;
using Stripe;
using System.Text;

namespace OShop.API
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // Add services to the container.

            builder.Services.AddControllers();
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
          
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                               policy =>
                               {
                                   policy.WithOrigins("http://localhost:5173") // Õœœ «·„Êﬁ⁄ «·„”„ÊÕ
                                         .AllowAnyMethod()
                                         .AllowAnyHeader(); // „Â„ Ãœ« Õ Ï  ”„Õ »‹ Content-Type
                               });
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));//create this object 
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<IBrandService, BrandsService>();
            builder.Services.AddScoped<IProductsServices, ProductsServices>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IReviewService, Services.REviews.ReviewService>();

            builder.Services.AddScoped<IPassResetCode,OShop.API.Services.PasswordReset. PassResetCode>();
            



            builder.Services.AddScoped<IDBinitilizer, DBinitilizer>();
            TypeAdapterConfig<Models.Review, ReviewResponse>.NewConfig()
               .Map(dest => dest.UserName, src => src.ApplicationUser.UserName);
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();//this bove code to sign any one of application user cuz need usermanager and signin manager and many others 
                                            // Add this before AddAuthentication()
                                            //to make the program use token not cokkies
            builder.Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//if one need to acces the end point and it doesn;t have auth it return 401

            }).AddJwtBearer(//check the token sedn with the request deos have correct singtuare or not
                Options =>
                {
                    Options.TokenValidationParameters = new()
                    {
                        ValidateIssuer=false,
                        ValidateAudience=false,
                        ValidateLifetime = true,//to check the expires of token is validate or not
                        ValidateIssuerSigningKey = true,//to compare the token send with my token
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lYlc929meK4cIhLGJfzE0zDWr65UrEi0")),//to check the signuare(the token that generate using this siginture only the token who provide it that mean if i use the same signtuare in another project maybe accure conflict when use the end point )
                    };
                });
            //to get secret key auto
            builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()||app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            /*this under code generate a scope this code will run inside it and this code exxute only one like scope */
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBinitilizer>();
            service.initilizer();

            app.Run();
        }
    }
}
