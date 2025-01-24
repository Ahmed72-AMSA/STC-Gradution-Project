using System.ComponentModel.DataAnnotations;

namespace STC.Models
{

 public class GoogleSignUpRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Gmail { get; set; }


       public string? NationalId { get; set; } ="Not entered yet";






    }

}