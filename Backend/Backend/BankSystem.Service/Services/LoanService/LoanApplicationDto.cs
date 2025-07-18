using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.LoanService
{
    public class LoanApplicationDto
    {


        public decimal IncomeAnnum { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTerm { get; set; }

        public int UserId{ get; set; }

        public string? Education { get; set; }
        public bool SelfEmployed { get; set; }

        public IFormFile NationalIdFile { get; set; }

    }
}
