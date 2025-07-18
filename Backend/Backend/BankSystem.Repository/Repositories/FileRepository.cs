using BankSystem.Data.Contexts;
using BankSystem.Data.Entities.Files;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
namespace BankSystem.Repository.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly BankingContext _dbContext;

        public FilesRepository(BankingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> IsFileBlacklistedAsync(string fileHash)
            => _dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == fileHash);

        public Task<bool> IsFileAlreadyUploadedAsync(string fileHash)
            => _dbContext.UploadedFiles.AnyAsync(f => f.FileHash == fileHash);

        public async Task AddUploadedFileAsync(UploadedFile file)
        {
            _dbContext.UploadedFiles.Add(file);
            await _dbContext.SaveChangesAsync();
        }

        public Task<UploadedFile> GetUploadedFileAsync(string fileHash)
            => _dbContext.UploadedFiles.FirstOrDefaultAsync(f => f.FileHash == fileHash);

        public async Task BlockFileHashAsync(string fileHash)
        {
            var blockedFile = new BlacklistedFile { FileHash = fileHash, BlockedDate = DateTime.UtcNow };
            _dbContext.BlacklistedFiles.Add(blockedFile);
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> IsHashBlockedAsync(string fileHash)
            => _dbContext.BlacklistedFiles.AnyAsync(b => b.FileHash == fileHash);
    }

}
