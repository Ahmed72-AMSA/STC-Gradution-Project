using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using STC.Models;

[Route("api/virustotal")]
[ApiController]
public class VirusTotalController : ControllerBase
{
    private readonly FileHashService _fileHashService;
    private readonly VirusTotalService _virusTotalService;

    public VirusTotalController()
    {
        _fileHashService = new FileHashService();
        _virusTotalService = new VirusTotalService("90481ab1eadf116026cf46a735a957b06b25cfaa231411def884d9f3068ccb34");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        using (var stream = file.OpenReadStream())
        {
            string fileHash = await _fileHashService.ComputeSHA256Async(stream);
            var resultJson = await _virusTotalService.CheckFileHashAsync(fileHash);
            Console.WriteLine($"VirusTotal API Response for {fileHash}: {resultJson}");

            if (string.IsNullOrWhiteSpace(resultJson) || (!resultJson.StartsWith("{") && !resultJson.StartsWith("[")))
            {
                return Ok(new
                {
                    hash = fileHash,
                    status = "Unknown",
                    message = "VirusTotal does not detect this file hash, recommended for SOC review."
                });
            }

            try
            {
                var virusTotalResponse = JsonConvert.DeserializeObject<VirusTotalResponse>(resultJson);
                var attributes = virusTotalResponse?.Data?.Attributes;

                if (attributes == null)
                    return Ok(new { hash = fileHash, status = "Unknown, recommended for SOC review." });

                var lastAnalysisResults = attributes.LastAnalysisResults;
                var detectionDetails = new List<object>();
                int maliciousCount = 0, harmlessCount = 0, suspiciousCount = 0, undetectedCount = 0;

                foreach (var engine in lastAnalysisResults)
                {
                    var category = engine.Value.Category;
                    if (category == "malicious") maliciousCount++;
                    else if (category == "harmless") harmlessCount++;
                    else if (category == "suspicious") suspiciousCount++;
                    else if (category == "undetected") undetectedCount++;

                    detectionDetails.Add(new
                    {
                        Engine = engine.Key,
                        Verdict = engine.Value.Result ?? "No result",
                        Category = category
                    });
                }

                string status = maliciousCount > 0 ? "Malicious file, immediate action required!" : "No detections found.";
                return Ok(new
                {
                    hash = fileHash,
                    status,
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
                    },
                    scan_results = detectionDetails
                });
            }
            catch (JsonException)
            {
                return Ok(new { hash = fileHash, error = "Failed to parse VirusTotal response." });
            }
        }
    }

    private string FormatUnixTimestamp(long? timestamp)
    {
        return timestamp.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            : "Unknown";
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
}














