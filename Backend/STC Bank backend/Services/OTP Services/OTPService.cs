// OTPService.cs
using System;
using System.Linq;

public class OTPService : IOTPService
{
    private static string _otp;
    private static DateTime _otpExpirationTime;

    public string GenerateOtp()
    {
        var guid = Guid.NewGuid(); 
        var otp = guid.ToString("N"); 

        var numericOtp = string.Join("", otp.Where(char.IsDigit));

        _otp = numericOtp.Length >= 6 ? numericOtp.Substring(0, 6) : numericOtp.PadRight(6, '0');

        _otpExpirationTime = DateTime.UtcNow.AddMinutes(5);

        return _otp;
    }

    public bool ValidateOtp(string otp)
    {
        if (otp == _otp && DateTime.UtcNow <= _otpExpirationTime)
        {
            _otp = null; 
            return true;
        }

        _otp = null; 
        return false;
    }
}
