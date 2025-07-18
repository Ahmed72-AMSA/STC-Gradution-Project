using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace BankSystem.Service.Services
{
    public class ChequeService : IChequeService
    {

        public string LastGeneratedChequeNumber { get; private set; }

        public async Task<byte[]> GenerateChequePdfAsync(
            string fromAccountName,
            string toName,
            string toBankName,
            string toAccountNumber,
            decimal amount)
        {
            // Generate a random 10-digit cheque number
            var random = new Random();
            LastGeneratedChequeNumber = $"{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;

            var gfx = XGraphics.FromPdfPage(page);

            // Background
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 250, 255)), 0, 0, page.Width, page.Height);

            // Watermark
            var watermarkFont = new XFont("Arial", 85, XFontStyle.Bold);
            var watermarkBrush = new XSolidBrush(XColor.FromArgb(20, 0, 0, 0));
            gfx.DrawString("BANK CHEQUE", watermarkFont, watermarkBrush,
                new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            // Bank Header
            var bankFont = new XFont("Georgia", 32, XFontStyle.Bold);
            gfx.DrawString("STC BANK", bankFont, XBrushes.MidnightBlue,
                new XPoint(page.Width / 2, 50), XStringFormats.TopCenter);

            var bankSubFont = new XFont("Georgia", 13, XFontStyle.Italic);
            gfx.DrawString("Member FDIC | Established 1907", bankSubFont, XBrushes.DimGray,
                new XPoint(page.Width / 2, 85), XStringFormats.TopCenter);

            // Title
            var titleFont = new XFont("Arial", 17, XFontStyle.Bold);
            gfx.DrawString("OFFICIAL BANK CHEQUE", titleFont, XBrushes.Black,
                new XPoint(page.Width / 2, 120), XStringFormats.TopCenter);

            // Bordered Section
            double sectionX = 60;
            double sectionY = 140;
            double sectionWidth = page.Width - 120;
            double sectionHeight = 280;
            double spacing = 30;

            DrawRoundedRect(gfx, new XPen(XColors.Gray, 1.5), XBrushes.White, sectionX, sectionY, sectionWidth, sectionHeight, 12);

            // Fonts
            var labelFont = new XFont("Arial", 12, XFontStyle.Bold);
            var valueFont = new XFont("Arial", 12, XFontStyle.Regular);
            var valueFontBlue = new XFont("Arial", 12, XFontStyle.BoldItalic);
            var valueFontGreen = new XFont("Arial", 13, XFontStyle.Bold);

            // Left details
            int leftX = 80;
            int currentY = (int)sectionY + 30;

            gfx.DrawString("Cheque Number:", labelFont, XBrushes.Black, leftX, currentY);
            gfx.DrawString(LastGeneratedChequeNumber, valueFont, XBrushes.Black, leftX + 160, currentY);

            currentY += (int)spacing;
            gfx.DrawString("Date:", labelFont, XBrushes.Black, leftX, currentY);
            gfx.DrawString(DateTime.Now.ToString("dd MMMM yyyy"), valueFont, XBrushes.Black, leftX + 160, currentY);

            currentY += (int)spacing;
            gfx.DrawString("Pay To:", labelFont, XBrushes.Black, leftX, currentY);
            gfx.DrawString(toName, valueFontBlue, XBrushes.Navy, leftX + 160, currentY);

            currentY += (int)spacing;
            gfx.DrawString("Bank:", labelFont, XBrushes.Black, leftX, currentY);
            gfx.DrawString(toBankName ?? "Any Bank", valueFont, XBrushes.Black, leftX + 160, currentY);

            currentY += (int)spacing;
            gfx.DrawString("Account Number:", labelFont, XBrushes.Black, leftX, currentY);
            gfx.DrawString(toAccountNumber ?? "N/A", valueFont, XBrushes.Black, leftX + 160, currentY);

            // Right details
            int rightX = (int)(page.Width / 2) + 50;
            int rightY = (int)sectionY + 30;

            gfx.DrawString("Account Holder:", labelFont, XBrushes.Black, rightX, rightY);
            gfx.DrawString(fromAccountName, valueFont, XBrushes.Black, rightX + 150, rightY);

            rightY += (int)spacing;
            gfx.DrawString("Amount (in Numbers):", labelFont, XBrushes.Black, rightX, rightY);
            gfx.DrawString(amount.ToString("C", CultureInfo.CreateSpecificCulture("en-US")),
                valueFontGreen, XBrushes.DarkGreen, rightX + 150, rightY);

            // Amount in words
            gfx.DrawString("Amount (in Words):", labelFont, XBrushes.Black, leftX, currentY + spacing + 10);
            gfx.DrawString(NumberToWords((int)amount) + " Dollars Only",
                valueFont, XBrushes.Black, new XRect(leftX + 180, currentY + spacing + 10, 600, 50), XStringFormats.TopLeft);

            // Signature line
            gfx.DrawLine(new XPen(XColors.Black, 1), rightX, sectionY + sectionHeight - 40, rightX + 180, sectionY + sectionHeight - 40);
            gfx.DrawString("Authorized Signature", valueFont, XBrushes.Black, rightX + 40, sectionY + sectionHeight - 25);

            // Security section
            var securityFont = new XFont("Arial", 9, XFontStyle.Italic);
            int secY = (int)(sectionY + sectionHeight + 20);
            gfx.DrawString("Security Features:", securityFont, XBrushes.DarkRed, leftX, secY);
            gfx.DrawString("• Microprinted signature line", securityFont, XBrushes.DarkRed, leftX, secY + 15);
            gfx.DrawString("• Watermark visible under light", securityFont, XBrushes.DarkRed, leftX, secY + 30);
            gfx.DrawString("• Embedded security thread", securityFont, XBrushes.DarkRed, leftX, secY + 45);

            // Footer
            var footerFont = new XFont("Arial", 8, XFontStyle.Italic);
            gfx.DrawString("Valid for 90 days from the issue date",
                footerFont, XBrushes.Gray, new XPoint(page.Width / 2, page.Height - 30), XStringFormats.Center);

            // Return PDF
            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        private string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[]
                {
                    "Zero", "One", "Two", "Three", "Four", "Five", "Six",
                    "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                    "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                    "Eighteen", "Nineteen"
                };

                var tensMap = new[]
                {
                    "Zero", "Ten", "Twenty", "Thirty", "Forty",
                    "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
                };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        private void DrawRoundedRect(XGraphics gfx, XPen pen, XBrush brush, double x, double y, double width, double height, double radius)
        {
            var path = new XGraphicsPath();
            double d = radius * 2;

            path.AddArc(x, y, d, d, 180, 90);
            path.AddLine(x + radius, y, x + width - radius, y);
            path.AddArc(x + width - d, y, d, d, 270, 90);
            path.AddLine(x + width, y + radius, x + width, y + height - radius);
            path.AddArc(x + width - d, y + height - d, d, d, 0, 90);
            path.AddLine(x + width - radius, y + height, x + radius, y + height);
            path.AddArc(x, y + height - d, d, d, 90, 90);
            path.AddLine(x, y + height - radius, x, y + radius);

            gfx.DrawPath(pen, brush, path);
        }
    }
}
