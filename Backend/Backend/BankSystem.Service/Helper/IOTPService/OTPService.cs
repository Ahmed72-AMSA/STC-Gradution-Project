using BankSystem.Service.Helper.IOTPService;
using System;
using System.Collections.Concurrent;

public class OTPService : IOTPService
{
    private static readonly ConcurrentDictionary<string, (string otp, DateTime expiry)> otpStore = new();

    private readonly TimeSpan _otpLifetime = TimeSpan.FromMinutes(5);

    public string GenerateOtp(string key = "default")
    {
        var otp = new Random().Next(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.Add(_otpLifetime);

        otpStore[key] = (otp, expiry);

        return otp;
    }

    public bool ValidateOtp(string otp, string key = "default")
    {
        if (!otpStore.ContainsKey(key))
            return false;

        var (storedOtp, expiry) = otpStore[key];

        if (DateTime.UtcNow > expiry)
        {
            otpStore.TryRemove(key, out _);
            return false;
        }

        var isValid = storedOtp == otp;

        if (isValid)
            otpStore.TryRemove(key, out _); 

        return isValid;
    }
}
