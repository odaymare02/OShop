﻿using System.ComponentModel.DataAnnotations;

namespace OShop.API.DTOs.Requests
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
