using Microsoft.AspNetCore.Identity;

namespace OShop.API.Models
{
    public enum ApplicationUserGender{// this like aprotoype that will accept only this things
        Male,Female
    }
    public class ApplicationUser:IdentityUser
    {
        public string ?State { get; set; }
        public string? City { get; set; }
        public string ?Street { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ApplicationUserGender Gender { get; set; }
        public DateTime BirthOfDate { get; set; }
    }
}
