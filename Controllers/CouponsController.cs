using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemberRewards.DTOs;
using MemberRewards.Services;
using System.Threading.Tasks;

namespace MemberRewards.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public CouponsController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemCoupon([FromBody] RedeemCouponDto redeemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, coupon) = await _memberService.RedeemPointsAsync(redeemDto.MemberId, redeemDto.PointsToRedeem);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, coupon });
        }
    }
}
