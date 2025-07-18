using BankSystem.Data.Contexts;
using BankSystem.Data.Entities.Files;
using BankSystem.Data.Entities.VirusTotal;
using BankSystem.Service.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Logging;
using BankSystem.Service.Services.FileHashService;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace BankSystem.Service.Services.FileScanService
{
    public class FileScanService : IFileScanService
    {
        private readonly BankingContext _dbContext;
        private readonly IFileHashService _fileHashService;
        private readonly VirusTotalService _virusTotalService;
        private readonly string _uploadPath;
        private readonly string _maliciousPath;
        private readonly ILogger<FileScanService> _logger;
        private readonly IVirusTotalReportService _reportService;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly HttpClient _client;

        public FileScanService(
            BankingContext dbContext,
            IFileHashService fileHashService,
            VirusTotalService virusTotalService,
            ILogger<FileScanService> logger,
            IVirusTotalReportService reportService,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _fileHashService = fileHashService;
            _virusTotalService = virusTotalService;
            _logger = logger;
            _reportService = reportService;
            _configuration = configuration;
            _apiKey = _configuration["VirusTotal:ApiKey"];
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("x-apikey", _apiKey);

            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _maliciousPath = Path.Combine(Directory.GetCurrentDirectory(), "SOCFiles", "malicious");

            EnsureDirectoryExists(_uploadPath);
            EnsureDirectoryExists(_maliciousPath);
        }

        public async Task<IActionResult> UploadAndScanFileAsync(IFormFile file, HttpRequest request)
        {
            if (file == null || file.Length == 0)
                return new BadRequestObjectResult(new { error = "No file uploaded." });

            string fileHash;
            using (var stream = file.OpenReadStream())
            {
                fileHash = await _fileHashService.ComputeSHA256Async(stream);
            }

            if (await _dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == fileHash))
                return new BadRequestObjectResult(new { error = "This file is blacklisted and cannot be uploaded." });

            if (await _dbContext.UploadedFiles.AnyAsync(f => f.FileHash == fileHash))
                return new ConflictObjectResult(new { error = "This file has already been uploaded and scanned." });

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(_uploadPath, fileName);
            await SaveFileAsync(file, filePath);

            var resultJson = await _virusTotalService.CheckFileHashAsync(fileHash);

            if (string.IsNullOrWhiteSpace(resultJson) || resultJson.Contains("NotFound") || (!resultJson.StartsWith("{") && !resultJson.StartsWith("[")))
            {
                _logger.LogWarning("VirusTotal response is empty or invalid for hash: {FileHash}. Response: {Response}", fileHash, resultJson);

                var fileType = Path.GetExtension(file.FileName).Trim('.').ToUpperInvariant();
                var scanResult = new UploadedFile
                {
                    FileName = file.FileName,
                    FileHash = fileHash,
                    FilePath = filePath,
                    Status = "Unknown and should be analyzed by SOC Members",
                    FileType = fileType,
                    FileSize = file.Length,
                    ScanDate = DateTime.UtcNow,
                    TotalEngines = 0,
                    MaliciousCount = 0,
                    HarmlessCount = 0,
                    SuspiciousCount = 0,
                    UndetectedCount = 0,
                    ScanDetailsJson = null
                };

                _dbContext.UploadedFiles.Add(scanResult);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    hash = fileHash,
                    status = "NotFound",
                    file_url = GetFileUrl(request, fileName, "uploads"),
                    message = "No VirusTotal report found. File stored for local analysis."
                });
            }

            return await ProcessVirusTotalResponse(resultJson, fileHash, fileName, filePath, file, request);
        }

        public async Task<IActionResult> GetScanDetailsAsync(string fileHash)
        {
            var scanResult = await _dbContext.UploadedFiles.FirstOrDefaultAsync(f => f.FileHash == fileHash);
            if (scanResult == null)
                return new NotFoundObjectResult(new { error = "No scan details found for the given file hash." });

            return new OkObjectResult(scanResult);
        }

        public async Task<IActionResult> BlockFileHashAsync(BlockedHashRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileHash))
                return new BadRequestObjectResult(new { error = "File hash is required." });

            if (await _dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == request.FileHash))
                return new ConflictObjectResult(new { error = "Hash is already blacklisted." });

            // Find any existing files with this hash
            var existingFiles = await _dbContext.UploadedFiles
                .Where(f => f.FileHash == request.FileHash)
                .ToListAsync();

            // Delete physical files from disk
            foreach (var file in existingFiles)
            {
                if (file.FilePath != null && System.IO.File.Exists(file.FilePath))
                {
                    try
                    {
                        System.IO.File.Delete(file.FilePath);
                        _logger.LogInformation("Deleted file from disk: {FilePath}", file.FilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete file from disk: {FilePath}", file.FilePath);
                    }
                }
            }

            // Remove files from database
            if (existingFiles.Any())
            {
                _dbContext.UploadedFiles.RemoveRange(existingFiles);
                _logger.LogInformation("Removed {Count} files with hash {Hash} from database", existingFiles.Count, request.FileHash);
            }

            // Add to blacklist
            var blockedFile = new BlacklistedFile
            {
                FileHash = request.FileHash,
                BlockedDate = DateTime.UtcNow
            };

            _dbContext.BlacklistedFiles.Add(blockedFile);

            // Save all changes in a single transaction
            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(new { 
                message = "File hash successfully blocked and associated files removed.", 
                hash = request.FileHash,
                filesRemoved = existingFiles.Count
            });
        }

        private async Task<IActionResult> ProcessVirusTotalResponse(
            string resultJson, string fileHash, string fileName, string filePath,
            IFormFile file, HttpRequest request)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<VirusTotalResponse>(resultJson);
                if (response?.Data?.Attributes == null)
                {
                    _logger.LogWarning("VirusTotal response has no attributes for hash: {FileHash}. Response: {Response}", fileHash, resultJson);
                    return new OkObjectResult(new
                    {
                        hash = fileHash,
                        status = "Unknown, recommended for SOC review.",
                        file_url = GetFileUrl(request, fileName, "uploads")
                    });
                }

                var attributes = response.Data.Attributes;
                var results = attributes.LastAnalysisResults;
                var (malicious, harmless, suspicious, undetected) = CountAnalysisResults(results);

                string status = malicious > 0 ? "Malicious" : "Clean";

                if (malicious > 0)
                {
                    string maliciousPath = Path.Combine(_maliciousPath, fileName);
                    System.IO.File.Move(filePath, maliciousPath);
                    filePath = maliciousPath;
                }

                var scanResult = new UploadedFile
                {
                    FileName = file.FileName,
                    FileHash = fileHash,
                    FilePath = filePath,
                    Status = status,
                    FileType = attributes.TypeDescription ?? "Unknown",
                    FileSize = attributes.Size ?? 0,
                    TotalEngines = results.Count,
                    MaliciousCount = malicious,
                    HarmlessCount = harmless,
                    SuspiciousCount = suspicious,
                    UndetectedCount = undetected,
                    ScanDate = DateTime.UtcNow,
                    ScanDetailsJson = JsonConvert.SerializeObject(new
                    {
                        file_type = attributes.TypeDescription ?? "Unknown",
                        file_size = FormatFileSize(attributes.Size),
                        first_submission = FormatUnixTimestamp(attributes.FirstSubmissionDate),
                        last_analysis = FormatUnixTimestamp(attributes.LastAnalysisDate),
                        times_submitted = attributes.TimesSubmitted,
                        analysis_results = results
                    }, Formatting.Indented)
                };

                _dbContext.UploadedFiles.Add(scanResult);
                var rowsAffected = await _dbContext.SaveChangesAsync();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("File scan result saved for hash: {FileHash}", fileHash);
                }
                else
                {
                    _logger.LogWarning("Failed to save scan result for hash: {FileHash}", fileHash);
                }

                return new OkObjectResult(new
                {
                    hash = fileHash,
                    status,
                    file_url = GetFileUrl(request, fileName, malicious > 0 ? "malicious" : "uploads"),
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
                        total_engines = results.Count,
                        malicious_count = malicious,
                        harmless_count = harmless,
                        suspicious_count = suspicious,
                        undetected_count = undetected
                    }
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse VirusTotal response for hash: {FileHash}. Response: {Response}", fileHash, resultJson);
                return new OkObjectResult(new
                {
                    hash = fileHash,
                    file_url = GetFileUrl(request, fileName, "uploads"),
                    error = "Failed to parse VirusTotal response."
                });
            }
        }

        private static (int malicious, int harmless, int suspicious, int undetected) CountAnalysisResults(Dictionary<string, VirusTotalAnalysis> results)
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
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private string GetFileUrl(HttpRequest request, string fileName, string folder)
        {
            if (request == null)
            {
                // fallback base url from config or hardcode (not ideal)
                string baseUrl = "https://yourdomain.com";
                return $"{baseUrl}/{folder}/{fileName}";
            }
            return $"{request.Scheme}://{request.Host}/{folder}/{fileName}";
        }


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

        public byte[] GenerateVirusTotalReport(string fileHash)
        {
            var fileDetails = _dbContext.UploadedFiles.FirstOrDefault(f => f.FileHash == fileHash);
            if (fileDetails == null)
                return null;

            return _reportService.GenerateVirusTotalReportPdf(fileDetails);
        }
    }
}
