using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities.Files
{   
    public class BlacklistedFile
    {
        public int Id { get; set; }
        public string? FileHash { get; set; }
        public DateTime BlockedDate { get; set; }
    }
}
