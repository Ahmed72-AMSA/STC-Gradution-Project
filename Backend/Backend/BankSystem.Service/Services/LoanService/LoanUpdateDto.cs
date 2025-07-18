using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.LoanService
{
    public class LoanUpdateDto
    {
        public decimal IncomeAnnum { get; set; }
        public int LoanTerm { get; set; }
        public string? Education { get; set; }
        public bool SelfEmployed { get; set; }
    }
}
