using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Service.Helper.IOTPService;
using BankSystem.Service.Helper.SMSServices;
using BankSystem.Data.Entities.Login;
using BankSystem.Service.Helper;
using BankSystem.Service.Services;
using Microsoft.AspNetCore.Mvc;

public class LoginService : ILoginService
{
    private readonly BankingContext _context;
    private readonly ISMSService _smsService;
    private readonly IOTPService _otpService;

    public LoginService(BankingContext context, ISMSService smsService, IOTPService otpService)
    {
        _context = context;
        _smsService = smsService;
        _otpService = otpService;
    }

    public async Task<ServiceResult> LoginAsync(LoginRequestDto loginRequest)
    {
        if (loginRequest == null)
            return ServiceResult.Fail("Login data is null.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
        if (user == null || user.HashedPassword != loginRequest.Password)
            return ServiceResult.Fail("Invalid Email or Password.");

        if (user.IsSuspended)
            return ServiceResult.Fail("Your account is suspended. Please contact support.");

        var otp = _otpService.GenerateOtp(); // 
        user.LoginOtp = otp;
        user.LoginOtpGeneratedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes.");

        return ServiceResult.Ok("OTP sent to your registered phone number.");
    }

    public async Task<ServiceResult> GoogleLoginAsync(GoogleLoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Gmail == request.Gmail);
        if (user == null)
            return ServiceResult.Fail("Invalid Gmail.");

        var otp = _otpService.GenerateOtp();
        user.LoginOtp = otp;
        user.LoginOtpGeneratedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        //_smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes.");
        Console.WriteLine($"[DEBUG] OTP for {user.Email}: {otp}");

        return ServiceResult.Ok("OTP sent to your registered phone number.");
    }

    public async Task<ServiceResult> FacebookLoginAsync(FacebookLoginDto request)
    {
        if (request == null)
            return ServiceResult.Fail("Facebook login data is null.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.FacebookId == request.FacebookId);
        if (user == null)
            return ServiceResult.Fail("Invalid Facebook ID.");

        var otp = _otpService.GenerateOtp();
        user.LoginOtp = otp;
        user.LoginOtpGeneratedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        //_smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes.");

        return ServiceResult.Ok("OTP sent to your registered phone number.");
    }

    public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Gmail))
            return ServiceResult.Fail("Gmail is required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Gmail);
        if (user == null)
            return ServiceResult.Fail("User with the provided Gmail not found.");

        var resetToken = _otpService.GenerateOtp();
        user.LoginOtp = resetToken;
        user.LoginOtpGeneratedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        //_smsService.Send(user.PhoneNumber, $"Your password reset token is: {resetToken}. Use it to reset your password.");

        return ServiceResult.Ok("Password reset token sent to your registered phone number.");
    }

    public async Task<ServiceResult> SuspendUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ServiceResult.Fail("User not found.");

        user.IsSuspended = true;
        await _context.SaveChangesAsync();

        return ServiceResult.Ok("User account suspended.");
    }

    public async Task<ServiceResult> UnsuspendUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ServiceResult.Fail("User not found.");

        user.IsSuspended = false;
        await _context.SaveChangesAsync();

        return ServiceResult.Ok("User account unsuspended.");
    }



    public async Task<ServiceResult> VerifyOtp(OtpVerificationRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Otp))
            return ServiceResult.Fail("OTP is required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginOtp == request.Otp);
        if (user == null)
            return ServiceResult.Fail("Invalid OTP.");

        if (user.LoginOtpGeneratedAt == null || user.LoginOtpGeneratedAt < DateTime.UtcNow.AddMinutes(-5))
            return ServiceResult.Fail("Expired OTP.");

        var userInfo = new UserInfoDto
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email
        };

        user.LoginOtp = null;
        user.LoginOtpGeneratedAt = null;
        if(user.Role == "Unknown")
        {
            user.Role = "User";
        }

        await _context.SaveChangesAsync();

        return ServiceResult.Ok("OTP verified successfully. Your role has been updated to User.", userInfo);
    }


}






