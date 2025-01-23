using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace STC.Models
{
    public class SignUp
    {
       public int? Id { get; set; }





        [Required(ErrorMessage = "UserName Is Required")]

        public string? UserName { get; set; }




        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string? Email { get; set; }




        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }





        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be exactly 11 digits.")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }











        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; } = "Not entered yet";


        public string? Gmail { get; set; }

        public string? FacebookId{get;set;}






    }
}
