using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STC.Models;
using STC.Data.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[Route("api/virustotal")]
[ApiController]
public class VirusTotalController : ControllerBase
{
    private readonly FileHashService _fileHashService;
    private readonly VirusTotalService _virusTotalService;
    private readonly string _uploadPath;
    private readonly string _maliciousPath;

    public VirusTotalController()
    {
        _fileHashService = new FileHashService();
        _virusTotalService = new VirusTotalService("90481ab1eadf116026cf46a735a957b06b25cfaa231411def884d9f3068ccb34");

        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _maliciousPath = Path.Combine(Directory.GetCurrentDirectory(), "SOCFiles", "malicious");

        EnsureDirectoryExists(_uploadPath);
        EnsureDirectoryExists(_maliciousPath);
    }







[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file, [FromServices] STCSystemDbContext dbContext)
{
    if (file == null || file.Length == 0)
        return BadRequest(new { error = "No file uploaded." });

    string fileHash;
    
    // Compute file hash
    using (var stream = file.OpenReadStream())
    {
        fileHash = await _fileHashService.ComputeSHA256Async(stream);
    }

    // Check if the file is blacklisted
    bool isBlacklisted = await dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == fileHash);
    if (isBlacklisted)
        return BadRequest(new { error = "This file is blacklisted and cannot be uploaded." });

    // Check if the file has already been uploaded
    bool isAlreadyUploaded = await dbContext.UploadedFiles.AnyAsync(f => f.FileHash == fileHash);
    if (isAlreadyUploaded)
        return Conflict(new { error = "This file has already been uploaded and scanned." });

    // Save file locally
    string fileName = $"{Guid.NewGuid()}_{file.FileName}";
    string filePath = Path.Combine(_uploadPath, fileName);
    await SaveFileAsync(file, filePath);

    // Check file hash with VirusTotal
    var resultJson = await _virusTotalService.CheckFileHashAsync(fileHash);
    Console.WriteLine($"VirusTotal API Response for {fileHash}: {resultJson}");

    if (string.IsNullOrWhiteSpace(resultJson) || (!resultJson.StartsWith("{") && !resultJson.StartsWith("[")))
    {
        return Ok(new { hash = fileHash, status = "Unknown", file_url = GetFileUrl(fileName, "uploads") });
    }

    return await ProcessVirusTotalResponse(resultJson, fileHash, fileName, filePath, file, dbContext);
}











    [HttpGet("details/{fileHash}")]
    public async Task<IActionResult> GetScanDetails(string fileHash, [FromServices] STCSystemDbContext dbContext)
    {
        var scanResult = await dbContext.UploadedFiles
            .FirstOrDefaultAsync(f => f.FileHash == fileHash);

        if (scanResult == null)
            return NotFound(new { error = "No scan details found for the given file hash." });

        return Ok(new
        {
            scanResult.FileName,
            scanResult.FileHash,
            scanResult.Status,
            scanResult.FilePath,
            scanResult.ScanDate,
            scanDetailsJson = scanResult.ScanDetailsJson
        });
    }





[HttpPost("block-hash")]
public async Task<IActionResult> BlockFileHash([FromBody] BlockedHashRequest request, [FromServices] STCSystemDbContext dbContext)
{
    if (string.IsNullOrWhiteSpace(request.FileHash))
        return BadRequest(new { error = "File hash is required." });

    // Check if the hash is already blacklisted
    bool exists = await dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == request.FileHash);
    if (exists)
        return Conflict(new { error = "Hash is already blacklisted." });

    // Save the hash to the blacklist
    var blockedFile = new BlacklistedFile { FileHash = request.FileHash, BlockedDate = DateTime.UtcNow };
    dbContext.BlacklistedFiles.Add(blockedFile);
    await dbContext.SaveChangesAsync();

    return Ok(new { message = "File hash successfully blocked.", hash = request.FileHash });
}



















    private async Task<IActionResult> ProcessVirusTotalResponse(
        string resultJson, string fileHash, string fileName, string filePath, IFormFile file, STCSystemDbContext dbContext)
    {
        try
        {
            var virusTotalResponse = JsonConvert.DeserializeObject<VirusTotalResponse>(resultJson);
            var attributes = virusTotalResponse?.Data?.Attributes;

            if (attributes == null)
            {
                return Ok(new { hash = fileHash, status = "Unknown, recommended for SOC review.", file_url = GetFileUrl(fileName, "uploads") });
            }

            var lastAnalysisResults = attributes.LastAnalysisResults;
            var (maliciousCount, harmlessCount, suspiciousCount, undetectedCount) = CountAnalysisResults(lastAnalysisResults);

            string status = maliciousCount > 0 ? "Malicious" : "Clean";

            if (maliciousCount > 0)
            {
                string maliciousFilePath = Path.Combine(_maliciousPath, fileName);
                System.IO.File.Move(filePath, maliciousFilePath);
                filePath = maliciousFilePath;
            }

            var scanResult = new UploadedFile
            {
                FileName = file.FileName,
                FileHash = fileHash,
                FilePath = filePath,
                Status = status,
                FileType = attributes.TypeDescription ?? "Unknown",
                FileSize = attributes.Size ?? 0,
                TotalEngines = lastAnalysisResults.Count,
                MaliciousCount = maliciousCount,
                HarmlessCount = harmlessCount,
                SuspiciousCount = suspiciousCount,
                UndetectedCount = undetectedCount,
                ScanDate = DateTime.UtcNow,
                ScanDetailsJson = JsonConvert.SerializeObject(new
                {
                    file_type = attributes.TypeDescription ?? "Unknown",
                    file_size = FormatFileSize(attributes.Size),
                    first_submission = FormatUnixTimestamp(attributes.FirstSubmissionDate),
                    last_analysis = FormatUnixTimestamp(attributes.LastAnalysisDate),
                    times_submitted = attributes.TimesSubmitted,
                    analysis_results = lastAnalysisResults
                }, Formatting.Indented)
            };

            Console.WriteLine($"Saving to DB: {scanResult.ScanDetailsJson}");
            dbContext.UploadedFiles.Add(scanResult);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                hash = fileHash,
                status,
                file_url = GetFileUrl(fileName, maliciousCount > 0 ? "malicious" : "uploads"),
                metadata = new
                {
                    file_type = attributes.TypeDescription ?? "Unknown",
                    file_size = FormatFileSize(attributes.Size),
                    first_submission = FormatUnixTimestamp(attributes.FirstSubmissionDate),
                    last_analysis = FormatUnixTimestamp(attributes.LastAnalysisDate),
                    times_submitted = attributes.TimesSubmitted
                },
                analysis_summary = new
                {
                    total_engines = lastAnalysisResults.Count,
                    malicious_count = maliciousCount,
                    harmless_count = harmlessCount,
                    suspicious_count = suspiciousCount,
                    undetected_count = undetectedCount
                }
            });
        }
        catch (JsonException)
        {
            return Ok(new { hash = fileHash, file_url = GetFileUrl(fileName, "uploads"), error = "Failed to parse VirusTotal response." });
        }
    }

    private static (int maliciousCount, int harmlessCount, int suspiciousCount, int undetectedCount) CountAnalysisResults(Dictionary<string, VirusTotalAnalysis> results)
    {
        int malicious = 0, harmless = 0, suspicious = 0, undetected = 0;

        foreach (var engine in results.Values)
        {
            switch (engine.Category)
            {
                case "malicious": malicious++; break;
                case "harmless": harmless++; break;
                case "suspicious": suspicious++; break;
                case "undetected": undetected++; break;
            }
        }

        return (malicious, harmless, suspicious, undetected);
    }




















    private static async Task SaveFileAsync(IFormFile file, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    private string GetFileUrl(string fileName, string folder) =>
        $"{Request.Scheme}://{Request.Host}/{folder}/{fileName}";

    private string FormatFileSize(long? size)
    {
        if (!size.HasValue) return "Unknown";
        double fileSize = size.Value;
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        while (fileSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            fileSize /= 1024;
        }
        return $"{fileSize:0.##} {sizes[order]}";
    }

    private string FormatUnixTimestamp(long? timestamp)
    {
        return timestamp.HasValue && timestamp > 0
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            : "Unknown";
    }
}
