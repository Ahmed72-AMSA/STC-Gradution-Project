using System.Security.Cryptography;

namespace BankSystem.Service.Services.FileHashService
{

    public class FileHashService : IFileHashService
    {
        public async Task<string> ComputeSHA256Async(Stream fileStream)
        {
            using var sha256 = SHA256.Create();
            var hash = await sha256.ComputeHashAsync(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}