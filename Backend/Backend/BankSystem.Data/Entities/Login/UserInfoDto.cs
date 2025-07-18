using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities.Login
{
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }

        public string? Role { get; set; }

        public string? Email { get; set; }

        public int? CIBIL_Score { get; set; }
    }
}
