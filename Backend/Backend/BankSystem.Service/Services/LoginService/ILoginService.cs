using BankSystem.Data.Entities.Login;
using BankSystem.Data.Entities;
using System.Threading.Tasks;
using BankSystem.Service.Helper;

namespace BankSystem.Service.Services
{
    public interface ILoginService
    {
        Task<ServiceResult> LoginAsync(LoginRequestDto request);
        Task<ServiceResult> GoogleLoginAsync(GoogleLoginDto request);
        Task<ServiceResult> FacebookLoginAsync(FacebookLoginDto request);
        Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<ServiceResult> SuspendUserAsync(int userId);
        Task<ServiceResult> UnsuspendUserAsync(int userId);
        Task<ServiceResult> VerifyOtp(OtpVerificationRequestDto request);

    }
}
