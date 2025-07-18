using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Files;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.IO;
using System.Text.Json;

namespace BankSystem.Service.Services.FileScanService
{
    public class VirusTotalReportService : IVirusTotalReportService
    {
        public byte[] GenerateVirusTotalReportPdf(UploadedFile fileDetails)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Colors
            var primary = XColor.FromArgb(0, 82, 147);
            var red = XColor.FromArgb(220, 53, 69);
            var yellow = XColor.FromArgb(255, 193, 7);
            var green = XColor.FromArgb(40, 167, 69);
            var gray = XColor.FromArgb(108, 117, 125);

            // Fonts
            var titleFont = new XFont("Arial", 22, XFontStyle.Bold);
            var headingFont = new XFont("Arial", 14, XFontStyle.Bold);
            var labelFont = new XFont("Arial", 11, XFontStyle.Bold);
            var valueFont = new XFont("Arial", 11, XFontStyle.Regular);
            var monoFont = new XFont("Arial", 10, XFontStyle.Regular);
            var footerFont = new XFont("Arial", 9, XFontStyle.Regular);

            double margin = 50;
            double currentY = margin;
            double contentWidth = page.Width - 2 * margin;

            // Header
            gfx.DrawRectangle(new XSolidBrush(primary), margin, currentY, contentWidth, 60);
            gfx.DrawString("VirusTotal Scan Report", titleFont, XBrushes.White,
                new XRect(margin, currentY + 18, contentWidth, 30), XStringFormats.Center);
            currentY += 80;

            // Section: File Information
            gfx.DrawString("File Information", headingFont, new XSolidBrush(primary), new XPoint(margin, currentY));
            currentY += 25;

            double col1 = margin;
            double col2 = page.Width / 2;

            double rowHeight = 22;

            DrawLabelAndValue(gfx, "File Name:", fileDetails.FileName, col1, currentY, labelFont, valueFont);
            DrawLabelAndValue(gfx, "File Type:", fileDetails.FileType, col2, currentY, labelFont, valueFont);
            currentY += rowHeight;

            DrawLabelAndValue(gfx, "File Size:", FormatFileSize(fileDetails.FileSize), col1, currentY, labelFont, valueFont);
            DrawLabelAndValue(gfx, "Scan Date:", fileDetails.ScanDate.ToString("yyyy-MM-dd HH:mm:ss"), col2, currentY, labelFont, valueFont);
            currentY += rowHeight;

            DrawLabelAndValue(gfx, "Status:", fileDetails.Status, col1, currentY, labelFont, new XSolidBrush(GetStatusColor(fileDetails.Status)));
            currentY += rowHeight;

            gfx.DrawString("File Hash:", labelFont, XBrushes.Gray, new XPoint(col1, currentY));
            currentY += 15;

            var hashBox = new XRect(col1, currentY, contentWidth, 20);
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 245, 245)), hashBox);
            gfx.DrawRectangle(XPens.LightGray, hashBox);
            gfx.DrawString(fileDetails.FileHash, monoFont, XBrushes.Black,
                new XRect(col1 + 5, currentY + 4, contentWidth, 20), XStringFormats.TopLeft);
            currentY += 35;

            // Section: Scan Results
            gfx.DrawString("Scan Results", headingFont, new XSolidBrush(primary), new XPoint(margin, currentY));
            currentY += 25;

            DrawLabelAndValue(gfx, "Malicious:", fileDetails.MaliciousCount.ToString(), col1, currentY, labelFont, new XSolidBrush(red));
            DrawLabelAndValue(gfx, "Suspicious:", fileDetails.SuspiciousCount.ToString(), col2, currentY, labelFont, new XSolidBrush(yellow));
            currentY += rowHeight;

            DrawLabelAndValue(gfx, "Clean:", fileDetails.UndetectedCount.ToString(), col1, currentY, labelFont, new XSolidBrush(green));
            DrawLabelAndValue(gfx, "Total Engines:", fileDetails.TotalEngines.ToString(), col2, currentY, labelFont, valueFont);
            currentY += 35;

            // Section: Additional Info
            if (!string.IsNullOrEmpty(fileDetails.ScanDetailsJson))
            {
                gfx.DrawString("Additional Information", headingFont, new XSolidBrush(primary), new XPoint(margin, currentY));
                currentY += 25;

                var scanDetails = JsonSerializer.Deserialize<JsonElement>(fileDetails.ScanDetailsJson);

                if (scanDetails.TryGetProperty("first_submission", out var firstSub))
                {
                    DrawLabelAndValue(gfx, "First Submission:", firstSub.GetString(), col1, currentY, labelFont, valueFont);
                    currentY += rowHeight;
                }

                if (scanDetails.TryGetProperty("last_analysis", out var lastAnalysis))
                {
                    DrawLabelAndValue(gfx, "Last Analysis:", lastAnalysis.GetString(), col1, currentY, labelFont, valueFont);
                    currentY += rowHeight;
                }

                if (scanDetails.TryGetProperty("times_submitted", out var timesSubmitted))
                {
                    DrawLabelAndValue(gfx, "Times Submitted:", timesSubmitted.ToString(), col1, currentY, labelFont, valueFont);
                    currentY += rowHeight;
                }
            }

            // Footer
            var footerY = page.Height - 50;
            gfx.DrawRectangle(new XSolidBrush(primary), margin, footerY, contentWidth, 30);
            gfx.DrawString($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont, XBrushes.White,
                new XRect(margin + 10, footerY + 8, contentWidth - 20, 20), XStringFormats.CenterLeft);
            gfx.DrawString("STC Bank Security Service", footerFont, XBrushes.White,
                new XRect(margin + 10, footerY + 8, contentWidth - 20, 20), XStringFormats.CenterRight);

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        private void DrawLabelAndValue(XGraphics gfx, string label, string value, double x, double y, XFont labelFont, XBrush valueBrush)
        {
            gfx.DrawString(label, labelFont, XBrushes.Gray, new XPoint(x, y));
            gfx.DrawString(value, labelFont, valueBrush, new XPoint(x + 100, y));
        }

        private void DrawLabelAndValue(XGraphics gfx, string label, string value, double x, double y, XFont labelFont, XFont valueFont)
        {
            gfx.DrawString(label, labelFont, XBrushes.Gray, new XPoint(x, y));
            gfx.DrawString(value, valueFont, XBrushes.Black, new XPoint(x + 100, y));
        }

        private string FormatFileSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = size;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private XColor GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "malicious" => XColor.FromArgb(220, 53, 69),
                "suspicious" => XColor.FromArgb(255, 193, 7),
                "clean" => XColor.FromArgb(40, 167, 69),
                _ => XColor.FromArgb(108, 117, 125)
            };
        }
    }
}
