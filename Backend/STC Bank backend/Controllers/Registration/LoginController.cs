// LoginController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STC.Models;
using STC.Data.Context;
using STC.Services;
using System;
using System.Threading.Tasks;

namespace STC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly STCSystemDbContext _context;
        private readonly ISMSService _smsService;
        private readonly IOTPService _otpService;

        public LoginController(STCSystemDbContext context, ISMSService smsService, IOTPService otpService)
        {
            _context = context;
            _smsService = smsService;
            _otpService = otpService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Login data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized("Invalid Email or Password.");
            }

        if (user.IsSuspended)  
        { 

        return Unauthorized("Your account is suspended. Please contact support.");
        
        
        }


            // Validate the password
            if (user.Password != loginRequest.Password)
            {
                return Unauthorized("Invalid Email or Password.");
            }

            var otp = _otpService.GenerateOtp();

            // Log the generated OTP for debugging
            Console.WriteLine($"Generated OTP: {otp} for Phone Number: {user.PhoneNumber}");

            _smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes");

            return Ok(new
            {
                Message = $"Login successful. OTP sent to your registered phone number."
            });
        }




        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp([FromBody] OtpVerificationRequest otpRequest)
        {
            if (otpRequest == null || string.IsNullOrWhiteSpace(otpRequest.Otp))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "OTP is required for verification."
                });
            }

            // Log the OTP that was sent for verification
            Console.WriteLine($"Verifying OTP: {otpRequest.Otp}");

            // Validate the OTP
            var isValid = _otpService.ValidateOtp(otpRequest.Otp);

            if (!isValid)
            {
                Console.WriteLine($"OTP {otpRequest.Otp} is invalid or expired.");
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid or expired OTP. Please try again."
                });
            }

            Console.WriteLine($"OTP {otpRequest.Otp} verified successfully.");
            return Ok(new
            {
                Success = true,
                Message = "OTP verified successfully."
            });
        }



[HttpPost("GoogleLogin")]
public async Task<IActionResult> GoogleLogin([FromBody] GoogleLogin googleLoginRequest)
{
    if (googleLoginRequest == null)
    {
        return BadRequest("Google login data is null.");
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Find the user by Gmail
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Gmail == googleLoginRequest.Gmail);

    if (user == null)
    {
        return Unauthorized("Invalid Gmail.");
    }

    // Generate a 6-digit OTP
    var otp = _otpService.GenerateOtp();

    // Log the generated OTP for debugging
    Console.WriteLine($"Generated OTP: {otp} for Phone Number: {user.PhoneNumber}");

    // Send the OTP via SMS
    _smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes");

    return Ok(new
    {
        Message = $"Login successful. OTP sent to your registered phone number."
    });
    
}




[HttpPost("FacebookLogin")]
public async Task<IActionResult> FacebookLogin([FromBody] FacebookLogin FBLogin)
{
    if (FBLogin == null)
    {
        return BadRequest("Facebook login data is null.");
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Find the user by Gmail
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.FacebookId == FBLogin.FacebookId);

    if (user == null)
    {
        return Unauthorized("Invalid Facebook ID.");
    }

    // Generate a 6-digit OTP
    var otp = _otpService.GenerateOtp();

    // Log the generated OTP for debugging
    Console.WriteLine($"Generated OTP: {otp} for Phone Number: {user.PhoneNumber}");

    // Send the OTP via SMS
    _smsService.Send(user.PhoneNumber, $"Your OTP is {otp}. It expires in 5 minutes");

    return Ok(new
    {
        Message = $"Login successful. OTP sent to your registered phone number."
    });






    }

[HttpPost("SuspendUser/{userId}")]
public async Task<IActionResult> SuspendUser(int userId)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    user.IsSuspended = true;
    await _context.SaveChangesAsync();

    return Ok(new { Message = "User account has been suspended successfully." });
}




[HttpPost("UnsuspendUser/{userId}")]
public async Task<IActionResult> UnsuspendUser(int userId)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    user.IsSuspended = false;
    await _context.SaveChangesAsync();

    return Ok(new { Message = "User account has been unsuspended successfully." });
}



[HttpPost("ForgotPassword")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
{
    if (forgotPasswordRequest == null || string.IsNullOrWhiteSpace(forgotPasswordRequest.Gmail))
    {
        return BadRequest("Gmail is required.");
    }

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Gmail == forgotPasswordRequest.Gmail);

    if (user == null)
    {
        return NotFound("User with the provided Gmail not found.");
    }

    var realPassword = user.Password; 

    // Send the real password via SMS
    _smsService.Send(user.PhoneNumber, $"Your password is: {realPassword}. Please use it to login.");

    return Ok(new
    {
        Message = "Your password has been sent to your registered phone number."
    });
}



}
}




