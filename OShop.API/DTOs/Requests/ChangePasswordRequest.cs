using System.ComponentModel.DataAnnotations;

namespace OShop.API.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}
