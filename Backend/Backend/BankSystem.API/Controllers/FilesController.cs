using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using BankSystem.Data.Entities.Files;
using BankSystem.Service.Services.FileScanService;
using BankSystem.Service.Services;

[Route("api/virustotal")]
[ApiController]
public class VirusTotalController : ControllerBase
{
    private readonly IFileScanService _fileScanService;

    public VirusTotalController(FileScanService fileScanService)
    {
        _fileScanService = fileScanService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var result = await _fileScanService.UploadAndScanFileAsync(file, Request);
        return result;
    }

    [HttpGet("details/{fileHash}")]
    public async Task<IActionResult> GetScanDetails(string fileHash)
    {
        var result = await _fileScanService.GetScanDetailsAsync(fileHash);
        return result;
    }

    [HttpPost("block-hash")]
    public async Task<IActionResult> BlockFileHash([FromBody] BlockedHashRequest request)
    {
        var result = await _fileScanService.BlockFileHashAsync(request);
        return result;
    }
}
