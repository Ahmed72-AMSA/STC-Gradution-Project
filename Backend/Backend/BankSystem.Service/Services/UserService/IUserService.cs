using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Login;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<(bool Success, string Message)> DeleteUserByFacebookIdAsync(string facebookId);
        Task<(bool, string)> CheckDuplicateAsync(string email, string? gmail = null, string? facebookId = null);
        Task<(bool Success, string Message, User? CreatedUser)> FacebookRegisterAsync(FacebookSignUpRequest request);
        Task<(bool Success, string Message)> UpdatePhoneAndIDByFacebookIdAsync(string facebookId, string phone, string nationalId);
        Task<(bool Success, string Message, User? CreatedUser)> GoogleSignUpAsync(GoogleSignUpRequest request);
        Task<(bool Success, string Message)> GoogleUpdatePhoneAndIDByUserIdAsync(int userId, string phone, string nationalId);
        Task<(bool Success, string Message)> RegisterUserAsync(UserRegisterDto dto);
        Task<(bool Success, string Message, UserInfoDto? Data)> GetUserBasicInfoAsync(int id);
        Task<(bool Success, string Message)> UpdateUserFieldsByIdAsync(int id, UserUpdateDto dto);
        Task<(bool Success, string Message, User? Data)> GetUserByNationalIdAsync(string nationalId);

    }

}
