using System.Threading.Tasks;

namespace BankSystem.Service.Services
{
    public interface IChequeService
    {
        Task<byte[]> GenerateChequePdfAsync(
            string fromAccountName,
            string toName,
            string toBankName,
            string toAccountNumber,
            decimal amount);

        string LastGeneratedChequeNumber { get; }

    }
}