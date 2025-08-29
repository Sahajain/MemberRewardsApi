using Microsoft.AspNetCore.Mvc;
using MemberRewards.DTOs;
using MemberRewards.Services;
using System.Threading.Tasks;

namespace MemberRewards.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IAuthService _authService;

        public MembersController(IMemberService memberService, IAuthService authService)
        {
            _memberService = memberService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var member = await _memberService.RegisterAsync(registerDto.MobileNumber);
            return Ok(new { message = "OTP sent successfully.", otp = member.Otp });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var member = await _memberService.GetMemberByMobileAsync(verifyDto.MobileNumber);
            if (member == null)
            {
                return BadRequest(new { message = "Member not found." });
            }

            // --- USE THE NEW SERVICE METHOD ---
            var isVerified = await _memberService.VerifyOtpAsync(member, verifyDto.Otp);

            if (!isVerified)
            {
                return BadRequest(new { message = "Invalid OTP or OTP expired." });
            }

            var token = _authService.GenerateJwtToken(member);

            return Ok(new AuthResponseDto { Token = token, MemberId = member.Id });
        }
    }
}
