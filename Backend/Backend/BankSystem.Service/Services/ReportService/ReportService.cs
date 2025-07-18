using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.IO;

namespace BankSystem.Service.Services.ReportService
{
    public class ReportService : IReportService
    {
        public byte[] GenerateTransactionReceiptPdf(TransactionReport report)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            // Modern Color Palette
            var primaryColor = XColor.FromArgb(0, 82, 147); 
            var secondaryColor = XColor.FromArgb(232, 241, 249); 
            var accentColor = XColor.FromArgb(255, 87, 34); 
            var backgroundColor = XColor.FromArgb(255, 255, 255);
            var textColor = XColor.FromArgb(51, 51, 51);
            var lightGray = XColor.FromArgb(240, 240, 240);

            // Fonts
            var fontTitle = new XFont("Calibri", 28, XFontStyle.Bold);
            var fontSubtitle = new XFont("Calibri", 18, XFontStyle.Bold);
            var fontSection = new XFont("Calibri", 14, XFontStyle.Bold);
            var fontContent = new XFont("Calibri", 11, XFontStyle.Regular);
            var fontBold = new XFont("Calibri", 11, XFontStyle.Bold);
            var fontSmall = new XFont("Calibri", 9, XFontStyle.Regular);
            var fontAmount = new XFont("Calibri", 24, XFontStyle.Bold);
            var fontSignature = new XFont("Brush Script MT", 18, XFontStyle.Bold);
            var fontFooter = new XFont("Calibri", 14, XFontStyle.Bold); // Larger font for footer

            double margin = 40;
            double width = page.Width - 2 * margin;
            double currentY = margin;

            // HEADER WITH LOGO PLACEHOLDER
            var headerHeight = 100;
            gfx.DrawRectangle(new XSolidBrush(primaryColor), margin, currentY, width, headerHeight);

            // Bank name and logo area
            gfx.DrawString("STC Bank", fontTitle, XBrushes.White, new XPoint(margin + 60, currentY + 60));


         
            currentY += headerHeight + 30;

            // RECEIPT TITLE
            gfx.DrawString("TRANSFER RECEIPT", fontSubtitle, new XSolidBrush(primaryColor),
                new XPoint(margin + width / 2 - 100, currentY));

            // Decorative elements
            gfx.DrawLine(new XPen(accentColor, 1.5), margin + width / 2 - 120, currentY + 25,
                margin + width / 2 - 20, currentY + 25);
            gfx.DrawLine(new XPen(accentColor, 1.5), margin + width / 2 + 120, currentY + 25,
                margin + width / 2 + 20, currentY + 25);

            currentY += 40;

            // TRANSACTION DETAILS SECTION
            var sectionRect = new XRect(margin, currentY, width, 200);
            gfx.DrawRoundedRectangle(new XPen(lightGray, 0.5), new XSolidBrush(secondaryColor), sectionRect, new XSize(5, 5));

            gfx.DrawString("Transaction Details", fontSection, new XSolidBrush(primaryColor),
                new XPoint(margin + 15, currentY + 20));

            double innerMargin = 20;
            double col1 = margin + innerMargin;
            double col2 = margin + width / 2;
            double rowHeight = 25;
            double boxPadding = 40;
            currentY += boxPadding;

            // Left column details
            DrawDetail(gfx, "Reference Number:", report.ReferenceNumber, col1, ref currentY, rowHeight, fontBold, fontContent);
            DrawDetail(gfx, "From Account:", report.AccountNumber, col1, ref currentY, rowHeight, fontBold, fontContent);
            DrawDetail(gfx, "Customer Name:", report.UserFullName, col1, ref currentY, rowHeight, fontBold, fontContent);

            // Right column details
            currentY = sectionRect.Y + boxPadding;
            DrawDetail(gfx, "Transaction Date:", report.Date.ToString("yyyy-MM-dd HH:mm:ss"), col2, ref currentY, rowHeight, fontBold, fontContent);
            DrawDetail(gfx, "Transaction Type:", report.TransactionType, col2, ref currentY, rowHeight, fontBold, fontContent);
            DrawDetail(gfx, "Status:", report.Status, col2, ref currentY, rowHeight, fontBold, fontContent);

            currentY = sectionRect.Y + sectionRect.Height + 20;

            // AMOUNT SECTION - HIGHLIGHTED
            var amountRect = new XRect(margin, currentY, width, 80);
            gfx.DrawRoundedRectangle(new XPen(accentColor, 1), new XSolidBrush(backgroundColor), amountRect, new XSize(5, 5));

            // Amount label and value
            gfx.DrawString("Amount Transferred", fontSection, new XSolidBrush(primaryColor),
                new XPoint(amountRect.X + 20, currentY + 30));

            gfx.DrawString(report.Amount.ToString("C"), fontAmount, new XSolidBrush(accentColor),
                new XPoint(amountRect.X + amountRect.Width - 120, currentY + 40));

            // Decorative accent
            gfx.DrawRectangle(new XSolidBrush(accentColor), amountRect.X, amountRect.Y, 8, amountRect.Height);

            currentY += amountRect.Height + 30;

            // BANK INFORMATION SECTION
            var bankSectionRect = new XRect(margin, currentY, width / 2 - 10, 140);
            gfx.DrawRoundedRectangle(new XPen(lightGray, 0.5), new XSolidBrush(secondaryColor), bankSectionRect, new XSize(5, 5));

            gfx.DrawString("Bank Information", fontSection, new XSolidBrush(primaryColor),
                new XPoint(margin + 15, currentY + 20));

            string[] bankInfoLines =
            {
                "STC Bank",
                "123 Financial District",
                "Cairo, Egypt",
                "Phone: (+20) 1021023089",
                "Email: service@stcbank.com",
                "www.stcbank.com"
            };

            double lineY = currentY + 50;
            foreach (var line in bankInfoLines)
            {
                gfx.DrawString(line, fontContent, new XSolidBrush(textColor),
                    new XPoint(margin + 25, lineY));
                lineY += 16;
            }

            // CUSTOMER SERVICE SECTION
            var serviceRect = new XRect(margin + width / 2 + 10, currentY, width / 2 - 10, 140);
            gfx.DrawRoundedRectangle(new XPen(lightGray, 0.5), new XSolidBrush(secondaryColor), serviceRect, new XSize(5, 5));

            gfx.DrawString("Customer Service", fontSection, new XSolidBrush(primaryColor),
                new XPoint(margin + width / 2 + 25, currentY + 20));

            string[] serviceLines =
            {
                "Available 24/7",
                "Phone: (+20) 19019",
                "Email: support@stcbank.com",
                "Live Chat: www.stcbank.com/support",
                "Branch Locator: www.stcbank.com/locations"
            };

            lineY = currentY + 50;
            foreach (var line in serviceLines)
            {
                gfx.DrawString(line, fontContent, new XSolidBrush(textColor),
                    new XPoint(margin + width / 2 + 25, lineY));
                lineY += 16;
            }

            currentY += serviceRect.Height + 30;

            // SIGNATURE AND STAMP AREA
            var signatureRect = new XRect(margin, currentY, width / 2 - 10, 80);
            gfx.DrawRoundedRectangle(new XPen(lightGray, 0.5), new XSolidBrush(backgroundColor), signatureRect, new XSize(5, 5));

            gfx.DrawLine(new XPen(textColor, 0.5), signatureRect.X + 20, currentY + 50,
                signatureRect.X + signatureRect.Width - 20, currentY + 50);
            gfx.DrawString("Authorized Signature", fontSignature, new XSolidBrush(primaryColor),
                new XPoint(signatureRect.X + 50, currentY + 70));

            // OFFICIAL STAMP
            var stampRect = new XRect(margin + width / 2 + 10, currentY, width / 2 - 10, 80);
            gfx.DrawRoundedRectangle(new XPen(lightGray, 0.5), new XSolidBrush(backgroundColor), stampRect, new XSize(5, 5));

            var innerStampRect = new XRect(stampRect.X + 20, stampRect.Y + 15, stampRect.Width - 40, 50);
            gfx.DrawEllipse(new XPen(accentColor, 1.5), innerStampRect);

            // Center "STC" in the stamp
            var stampTextSize = gfx.MeasureString("STC", new XFont("Calibri", 10, XFontStyle.Bold));
            gfx.DrawString("STC", new XFont("Calibri", 10, XFontStyle.Bold),
                new XSolidBrush(accentColor),
                new XPoint(innerStampRect.X + innerStampRect.Width / 2 - stampTextSize.Width / 2,
                          innerStampRect.Y + innerStampRect.Height / 2 - stampTextSize.Height / 2 + 5));

            gfx.DrawString(DateTime.Now.ToString("MMM dd, yyyy"), new XFont("Calibri", 8, XFontStyle.Regular),
                new XSolidBrush(textColor), new XPoint(innerStampRect.X + 15, innerStampRect.Y + 40));

            currentY += stampRect.Height + 30;

            // FOOTER (with increased height and better text positioning)
            var footerRect = new XRect(margin, currentY, width, 80); // Increased height from 60 to 80
            gfx.DrawRectangle(new XSolidBrush(primaryColor), footerRect);

            // Center the thank you message vertically in the taller footer
            var thankYouSize = gfx.MeasureString("Thank you for banking with STC Bank", fontFooter);
            gfx.DrawString("Thank you for banking with STC Bank", fontFooter, XBrushes.White,
                new XPoint(footerRect.X + footerRect.Width / 2 - thankYouSize.Width / 2,
                          footerRect.Y + footerRect.Height / 2 - thankYouSize.Height / 2));

            gfx.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontSmall, XBrushes.White,
                new XPoint(footerRect.X + footerRect.Width - 120, footerRect.Y + footerRect.Height - 20));

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        private void DrawDetail(XGraphics gfx, string label, string value, double x, ref double y, double rowHeight,
            XFont labelFont, XFont valueFont)
        {
            gfx.DrawString(label, labelFont, XBrushes.Gray, new XPoint(x, y));
            gfx.DrawString(value, valueFont, XBrushes.Black, new XPoint(x + 150, y));
            y += rowHeight;
        }
    }
}