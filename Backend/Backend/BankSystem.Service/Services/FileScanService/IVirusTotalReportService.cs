using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Files;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.FileScanService
{
    public interface IVirusTotalReportService
    {
        byte[] GenerateVirusTotalReportPdf(UploadedFile fileDetails);
    }
} 