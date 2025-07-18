using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    using System.ComponentModel.DataAnnotations;


        public class UserRegisterDto
        {
            [Required(ErrorMessage = "UserName Is Required")]
            public string? UserName { get; set; }

            [Required(ErrorMessage = "Email Is Required")]
            [EmailAddress(ErrorMessage = "Invalid Format For Email")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Password Is Required")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$",
                ErrorMessage = "Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.")]
            [DataType(DataType.Password)]
            public string HashedPassword { get; set; } = string.Empty;

            public string? Role { get; set; } = "Unknown";

            [Required(ErrorMessage = "Phone Number is required")]
            [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be exactly 11 digits.")]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
         public string? NationalID { get; set; }






    }
}


