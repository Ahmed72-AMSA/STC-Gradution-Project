using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.UserService
{
    public class UserUpdateDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalID { get; set; } = string.Empty;
        public bool IsSuspended { get; set; }
    }

}