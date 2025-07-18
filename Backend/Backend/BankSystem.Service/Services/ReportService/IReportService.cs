using BankSystem.Data.Entities;

namespace BankSystem.Service.Services.ReportService
{
    public interface IReportService
    {
        byte[] GenerateTransactionReceiptPdf(TransactionReport report);
    }
}