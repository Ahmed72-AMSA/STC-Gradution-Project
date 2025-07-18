using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Login;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _repository.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<bool> CreateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.NationalID) || user.NationalID.Length != 14)
                throw new ArgumentException("National ID is required and must be exactly 14 digits.");

            user.PhoneNumber = NormalizePhoneNumber(user.PhoneNumber);
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            return true;
        }

  
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            await _repository.DeleteAsync(user);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<(bool, string)> CheckDuplicateAsync(string email, string? gmail = null, string? facebookId = null)
        {
            if (await _repository.GetByEmailAsync(email) is not null)
                return (true, "Email already exists.");

            if (!string.IsNullOrEmpty(gmail) && await _repository.GetByGmailAsync(gmail) is not null)
                return (true, "Gmail already registered.");

            if (!string.IsNullOrEmpty(facebookId) && await _repository.GetByFacebookIdAsync(facebookId) is not null)
                return (true, "Facebook ID already registered.");

            return (false, string.Empty);
        }

        public async Task<(bool Success, string Message, User? CreatedUser)> FacebookRegisterAsync(FacebookSignUpRequest request)
        {
            var (exists, reason) = await CheckDuplicateAsync(request.Email, request.Gmail, request.FacebookId);
            if (exists)
                return (false, reason, null);

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Gmail = request.Gmail,
                FacebookId = request.FacebookId,
                Role = "Unknown",
                HashedPassword = "facebook_registration",
                PhoneNumber = String.Empty,
                UserCreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newUser);
            await _repository.SaveChangesAsync();

            return (true, "User created successfully", newUser);
        }

        public async Task<(bool Success, string Message)> UpdatePhoneAndIDByFacebookIdAsync(string facebookId, string phone, string nationalId)
        {
            var user = await _repository.GetByFacebookIdAsync(facebookId);
            if (user == null)
                return (false, "User not found.");

            user.PhoneNumber = NormalizePhoneNumber(phone);
            user.NationalID = nationalId;

            await _repository.SaveChangesAsync();
            return (true, "Phone number and National ID updated.");
        }


        public async Task<(bool Success, string Message)> DeleteUserByFacebookIdAsync(string facebookId)
        {
            var user = await _repository.GetByFacebookIdAsync(facebookId);
            if (user == null)
                return (false, "User not found.");

            await _repository.DeleteAsync(user);
            await _repository.SaveChangesAsync();

            return (true, "User deleted successfully.");
        }


        public async Task<(bool Success, string Message, User? CreatedUser)> GoogleSignUpAsync(GoogleSignUpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Gmail) || string.IsNullOrWhiteSpace(request.UserName))
                return (false, "Username, Email, and Gmail are required.", null);

            var (exists, reason) = await CheckDuplicateAsync(request.Email, request.Gmail);
            if (exists)
                return (false, reason, null);

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Gmail = request.Gmail,
                Role = "Unknown",
                HashedPassword = "google_registration",
                PhoneNumber = string.Empty,
                NationalID = null,
                UserCreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newUser);
            await _repository.SaveChangesAsync();

            return (true, "User registered with Google successfully.", newUser);
        }



        public async Task<(bool Success, string Message)> GoogleUpdatePhoneAndIDByUserIdAsync(int userId, string phone, string nationalId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            user.PhoneNumber = NormalizePhoneNumber(phone);
            user.NationalID = nationalId;

            await _repository.SaveChangesAsync();
            return (true, "Phone number and National ID updated.");
        }


        public async Task<(bool Success, string Message)> RegisterUserAsync(UserRegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.HashedPassword) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.NationalID))
            {
                return (false, "All required fields must be filled.");
            }

            var (exists, reason) = await CheckDuplicateAsync(dto.Email!);
            if (exists)
                return (false, reason);

            var newUser = new User
            {
                UserName = dto.UserName!,
                Email = dto.Email!,
                HashedPassword = dto.HashedPassword,
                PhoneNumber = NormalizePhoneNumber(dto.PhoneNumber),
                Role = dto.Role ?? "Unknown",
                NationalID = dto.NationalID,
                UserCreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newUser);
            await _repository.SaveChangesAsync();

            return (true, "User registered successfully.");
        }

        public async Task<(bool Success, string Message, UserInfoDto? Data)> GetUserBasicInfoAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found.", null);

            var dto = new UserInfoDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Role = user.Role,
                Email = user.Email,
                CIBIL_Score = user.CIBIL_Score,
                
            };

            return (true, "User found.", dto);
        }


        public async Task<(bool Success, string Message)> UpdateUserFieldsByIdAsync(int id, UserUpdateDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found.");

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.PhoneNumber = NormalizePhoneNumber(dto.PhoneNumber);
            user.NationalID = dto.NationalID;
            user.IsSuspended = dto.IsSuspended;

            await _repository.SaveChangesAsync();
            return (true, "User updated successfully.");
        }


        public async Task<(bool Success, string Message, User? Data)> GetUserByNationalIdAsync(string nationalId)
        {
            var user = await _repository.GetByNationalIdAsync(nationalId);
            if (user == null)
                return (false, "User not found.", null);

            return (true, "User found.", user);
        }





        private string NormalizePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber?.Trim() ?? string.Empty;
            return phoneNumber.StartsWith("+2") ? phoneNumber : $"+2{phoneNumber}";
        }
    }
}
