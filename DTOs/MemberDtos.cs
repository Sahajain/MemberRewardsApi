using System.ComponentModel.DataAnnotations;

namespace MemberRewards.DTOs
{
    public class RegisterDto
    {
        [Required]
        [Phone]
        public string MobileNumber { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required]
        [Phone]
        public string MobileNumber { get; set; }
        [Required]
        public string Otp { get; set; }
    }

    public class AddPointsDto
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public decimal PurchaseAmount { get; set; }
    }

    public class RedeemCouponDto
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        public int PointsToRedeem { get; set; } // e.g., 500 or 1000
    }
}