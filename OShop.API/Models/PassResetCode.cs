using System.Runtime.CompilerServices;

namespace OShop.API.Models
{
    public class PassResetCode
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public String code { get; set; }
        public DateTime ExpirationCode { get; set; }
    }
}
