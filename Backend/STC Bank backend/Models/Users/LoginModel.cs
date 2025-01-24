using System.ComponentModel.DataAnnotations;

namespace STC.Models
{


public class LoginRequest
{
    [Required(ErrorMessage = "Email Is Required")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }




      [Required(ErrorMessage = "Password Is Required")]
      [DataType(DataType.Password)]
    public string? Password { get; set; }
}

}