using System.ComponentModel.DataAnnotations;

namespace STC.Models
{
    public class SignUp
    {
       public int Id { get; set; }





        [Required(ErrorMessage = "UserName Is Required")]

        public string? UserName { get; set; }


        public string? ChatRoom { get; set; } 

        public string? ConnectionId { get; set; }
    



        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string? Email { get; set; }



        public string? Role { get; set; } = "User";




        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }





        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be exactly 11 digits.")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }





        public string? Gmail { get; set; }
        public bool IsSuspended { get; set; }

        public string? FacebookId{get;set;}


        public virtual Complain? Complain { get; set; } 










    }
}
