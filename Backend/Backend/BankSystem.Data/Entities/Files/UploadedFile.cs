using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities.Files
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileHash { get; set; }
        public string? FilePath { get; set; }
        public string? Status { get; set; } = "No Status";
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public int TotalEngines { get; set; }
        public int MaliciousCount { get; set; }
        public int HarmlessCount { get; set; }
        public int SuspiciousCount { get; set; }
        public int UndetectedCount { get; set; }
        public DateTime ScanDate { get; set; }
        public string? ScanDetailsJson { get; set; }
    }
}
