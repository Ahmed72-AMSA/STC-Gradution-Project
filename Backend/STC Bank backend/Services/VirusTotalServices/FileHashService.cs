using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class FileHashService
{
    public async Task<string> ComputeSHA256Async(Stream fileStream)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = await sha256.ComputeHashAsync(fileStream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
