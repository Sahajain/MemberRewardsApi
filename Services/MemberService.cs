using MemberRewards.Data;
using MemberRewards.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MemberRewards.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;

        public MemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Member> RegisterAsync(string mobileNumber)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.MobileNumber == mobileNumber);
            if (member == null)
            {
                member = new Member { MobileNumber = mobileNumber, IsVerified = false, TotalPoints = 0 };
                _context.Members.Add(member);
            }

            member.Otp = new Random().Next(100000, 999999).ToString();
            member.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<Member?> GetMemberByMobileAsync(string mobileNumber)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.MobileNumber == mobileNumber);
        }

        // --- ADD THIS NEW METHOD IMPLEMENTATION ---
        public async Task<bool> VerifyOtpAsync(Member member, string otp)
        {
            if (member.Otp != otp || member.OtpExpiry < DateTime.UtcNow)
            {
                return false;
            }

            member.IsVerified = true;
            member.Otp = null; // Clear OTP after verification
            member.OtpExpiry = null;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AddPointsAsync(int memberId, decimal purchaseAmount)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null || !member.IsVerified) return false;

            int pointsToAdd = (int)(purchaseAmount / 100) * 10;
            if (pointsToAdd > 0)
            {
                member.TotalPoints += pointsToAdd;
                _context.Points.Add(new Point { MemberId = memberId, PointsAdded = pointsToAdd });
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<(bool success, string message, Coupon? coupon)> RedeemPointsAsync(int memberId, int pointsToRedeem)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null || !member.IsVerified) return (false, "Member not found or not verified.", null);

            if (member.TotalPoints < pointsToRedeem) return (false, "Insufficient points.", null);

            int couponValue = 0;
            if (pointsToRedeem == 500) couponValue = 50;
            else if (pointsToRedeem == 1000) couponValue = 100;
            else return (false, "Invalid points amount for redemption.", null);

            member.TotalPoints -= pointsToRedeem;
            var coupon = new Coupon
            {
                MemberId = memberId,
                CouponCode = $"REDEEM-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Value = couponValue
            };
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return (true, "Coupon redeemed successfully.", coupon);
        }
    }
}