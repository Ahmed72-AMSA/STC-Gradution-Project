using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankSystem.Data.Entities.Loans;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Data.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Gmail), IsUnique = true)]
    [Index(nameof(FacebookId), IsUnique = true)]
    [Index(nameof(NationalID), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid format for Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$",
            ErrorMessage = "Password must meet complexity requirements")]
        public string HashedPassword { get; set; } = string.Empty;

        public string? Role { get; set; } = "User";

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalID { get; set; }

        public DateTime? UserCreatedAt { get; set; }

        public string? FacebookId { get; set; }
        public string? Gmail { get; set; }

        public string? LoginOtp { get; set; }
        public DateTime? LoginOtpGeneratedAt { get; set; }

        public string? OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }

        public bool IsSuspended { get; set; }
        public string? SuspensionReason { get; set; }
        public DateTime? SuspensionDate { get; set; }

   

        public string? ChatRoom { get; set; }
        public string? ConnectionId { get; set; }

        // Navigation properties
        public Account Account { get; set; }
        public List<Complain> Complains { get; set; } = new();
        public List<Cheque> Cheques { get; set; } = new();
        public int CIBIL_Score { get; set; } = 600;

        public List<Loan> Loans { get; set; } = new();

    }
}