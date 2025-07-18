using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Helper
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public static ServiceResult Ok(string message, object? data = null) =>
            new() { Success = true, Message = message, Data = data };

        public static ServiceResult Fail(string message) =>
            new() { Success = false, Message = message };
    }

}
