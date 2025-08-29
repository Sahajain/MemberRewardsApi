namespace MemberRewards.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int PointsAdded { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public Member Member { get; set; }
    }
}