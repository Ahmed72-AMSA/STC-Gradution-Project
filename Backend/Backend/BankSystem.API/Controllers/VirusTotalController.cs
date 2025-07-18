using Microsoft.AspNetCore.Mvc;
using BankSystem.Service.Services;

namespace BankSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VirusTotalController : ControllerBase
    {
        private readonly IFileScanService _fileScanService;

        public VirusTotalController(IFileScanService fileScanService)
        {
            _fileScanService = fileScanService;
        }

        [HttpGet("report/{fileHash}")]
        public IActionResult DownloadReport(string fileHash)
        {
            var pdfBytes = _fileScanService.GenerateVirusTotalReport(fileHash);
            if (pdfBytes == null)
                return NotFound("File details not found.");

            return File(pdfBytes, "application/pdf", $"VirusTotal_Report_{fileHash}.pdf");
        }
    }
} 