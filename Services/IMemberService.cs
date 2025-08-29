using MemberRewards.Models;
using System.Threading.Tasks;

namespace MemberRewards.Services
{
    public interface IMemberService
    {
        Task<Member> RegisterAsync(string mobileNumber);
        Task<Member?> GetMemberByMobileAsync(string mobileNumber);
        // --- ADD THIS NEW METHOD ---
        Task<bool> VerifyOtpAsync(Member member, string otp);
        Task<bool> AddPointsAsync(int memberId, decimal purchaseAmount);
        Task<(bool success, string message, Coupon? coupon)> RedeemPointsAsync(int memberId, int pointsToRedeem);
    }
}