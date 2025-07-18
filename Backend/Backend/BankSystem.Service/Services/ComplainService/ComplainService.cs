using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.ComplainService
{
    public class ComplainService : IComplainService
    {
        private readonly IComplainRepository _repository;

        public ComplainService(IComplainRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ComplainDto>> GetAllComplainsAsync()
        {
            var complains = await _repository.GetAllAsync();
            return complains.Select(MapToDto).ToList();
        }

        public async Task<ComplainDto?> GetComplainByIdAsync(int id)
        {
            var complain = await _repository.GetByIdAsync(id);
            return complain == null ? null : MapToDto(complain);
        }

        public async Task<ComplainDto> CreateComplainAsync(ComplainDto complainDto)
        {
            var complain = new Complain
            {
                Describtion = complainDto.Describtion,
                Recipient = complainDto.Recipient,
                Solved = complainDto.Solved,
                Timestamp = DateTime.UtcNow,
                EndDate = complainDto.EndDate,
                UserId = complainDto.UserId
            };

            await _repository.AddAsync(complain);

            // Populate generated ID and Timestamp
            return MapToDto(complain);
        }

        public async Task<bool> UpdateComplainAsync(int id, ComplainDto updatedDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Describtion = updatedDto.Describtion;
            existing.Recipient = updatedDto.Recipient;
            existing.Solved = updatedDto.Solved;
            existing.EndDate = updatedDto.EndDate;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteComplainAsync(int id)
        {
            var complain = await _repository.GetByIdAsync(id);
            if (complain == null) return false;

            await _repository.DeleteAsync(complain);
            return true;
        }

        private ComplainDto MapToDto(Complain complain)
        {
            return new ComplainDto
            {
                Id = complain.Id,
                Describtion = complain.Describtion,
                Recipient = complain.Recipient,
                Solved = complain.Solved,
                Timestamp = complain.Timestamp,
                EndDate = complain.EndDate,
                UserId = complain.UserId
            };
        }
    }
}
