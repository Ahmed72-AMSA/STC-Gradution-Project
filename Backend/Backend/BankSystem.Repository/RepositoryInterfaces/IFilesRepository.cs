using BankSystem.Data.Entities.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IFilesRepository
    {

        Task<bool> IsFileBlacklistedAsync(string fileHash);
        Task<bool> IsFileAlreadyUploadedAsync(string fileHash);
        Task AddUploadedFileAsync(UploadedFile file);
        Task<UploadedFile> GetUploadedFileAsync(string fileHash);
        Task BlockFileHashAsync(string fileHash);
        Task<bool> IsHashBlockedAsync(string fileHash);
    }

}