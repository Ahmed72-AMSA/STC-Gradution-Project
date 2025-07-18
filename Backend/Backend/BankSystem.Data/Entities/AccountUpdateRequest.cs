using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class AccountUpdateRequest
    {
        public decimal NewBalance { get; set; }
        public string? AccountStatus { get; set; }
    }
}
