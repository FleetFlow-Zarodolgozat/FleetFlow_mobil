using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace mobil.Models
{
    public class Login
    {
        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [Required]
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    public class ForgotPassword
    {
        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
