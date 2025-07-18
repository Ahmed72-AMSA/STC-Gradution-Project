using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Helper.IOTPService
{
    public interface IOTPService
    {
        string GenerateOtp(string key = "default");
        bool ValidateOtp(string otp, string key = "default");
    }

}
