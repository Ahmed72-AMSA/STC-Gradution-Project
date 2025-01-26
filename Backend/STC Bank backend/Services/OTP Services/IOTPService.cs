// IOTPService.cs
public interface IOTPService
{
    string GenerateOtp();
    bool ValidateOtp(string otp);
}
