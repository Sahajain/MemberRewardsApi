namespace MemberRewards.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string CouponCode { get; set; }
        public int Value { get; set; }
        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
        public Member Member { get; set; }
    }
}