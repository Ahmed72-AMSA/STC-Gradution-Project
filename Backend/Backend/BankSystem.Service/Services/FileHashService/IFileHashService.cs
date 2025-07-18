

namespace BankSystem.Service.Services.FileHashService
{
    public interface IFileHashService
    {
        Task<string> ComputeSHA256Async(Stream fileStream);
    }
}
