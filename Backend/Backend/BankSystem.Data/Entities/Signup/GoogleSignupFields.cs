using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities.Signup
{ 
public class GoogleUpdatePhoneAndIDRequest
{ 


    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
    public string NationalID { get; set; } = string.Empty;
}

}