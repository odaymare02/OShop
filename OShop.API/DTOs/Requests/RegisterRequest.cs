using OShop.API.Models;
using System.ComponentModel.DataAnnotations;
using OShop.API.Validations;
namespace OShop.API.DTOs.Requests
{
    public class RegisterRequest
    {
        [MinLength(3)]
        public string FirstName { get; set; }
        [MinLength(4)]
        public string LastName { get; set; }
        [MinLength(6)]
        public string? UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare(nameof(Password),ErrorMessage ="passwords doesn't match")]
        public string ConfirmPassword { get; set; }
        public ApplicationUserGender Gender { get; set; }
        [Over18Year(18)]
        public DateTime BirthOfDate { get; set; }
    }
}
