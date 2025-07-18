using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class FacebookSignUpRequest
    {
        [Required(ErrorMessage = "Facebook ID is required")]
        public string FacebookId { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        // Gmail is optional, but must be unique if provided
        public string? Gmail { get; set; }

        public string Role { get; set; } = "Unknown";

    }
}
