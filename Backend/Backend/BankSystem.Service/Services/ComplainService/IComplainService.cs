using BankSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.ComplainService
{
    public interface IComplainService
    {
        Task<List<ComplainDto>> GetAllComplainsAsync();
        Task<ComplainDto?> GetComplainByIdAsync(int id);
        Task<ComplainDto> CreateComplainAsync(ComplainDto complainDto);
        Task<bool> UpdateComplainAsync(int id, ComplainDto updatedDto);
        Task<bool> DeleteComplainAsync(int id);
    }
}
