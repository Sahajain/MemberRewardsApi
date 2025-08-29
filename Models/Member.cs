using System.ComponentModel.DataAnnotations;

namespace MemberRewards.Models
{
    public class Member
    {
        public int Id { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsVerified { get; set; }
        public int TotalPoints { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}