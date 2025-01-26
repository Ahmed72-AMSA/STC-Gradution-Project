// OTPService.cs
using System;
using System.Linq;

public class OTPService : IOTPService
{
    private static string _otp;
    private static DateTime _otpExpirationTime;

    // Generates a 6-digit numeric OTP using GUID
    public string GenerateOtp()
    {
        var guid = Guid.NewGuid(); // Generate a new GUID
        var otp = guid.ToString("N"); // Convert it to a string without hyphens

        // Extract only numeric characters
        var numericOtp = string.Join("", otp.Where(char.IsDigit));

        // Ensure we get exactly 6 digits
        _otp = numericOtp.Length >= 6 ? numericOtp.Substring(0, 6) : numericOtp.PadRight(6, '0');

        // Set OTP expiration time (5 minutes)
        _otpExpirationTime = DateTime.UtcNow.AddMinutes(5);

        return _otp;
    }

    // Validates the OTP
    public bool ValidateOtp(string otp)
    {
        // Check if the OTP is valid and not expired
        if (otp == _otp && DateTime.UtcNow <= _otpExpirationTime)
        {
            _otp = null; // Clear OTP after validation
            return true;
        }

        _otp = null; // Clear OTP after expiration or invalidity
        return false;
    }
}
