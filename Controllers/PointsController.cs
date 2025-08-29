using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemberRewards.DTOs;
using MemberRewards.Services;
using System.Threading.Tasks;
using MemberRewards.Data;
using Microsoft.EntityFrameworkCore;

namespace MemberRewards.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect these endpoints
    public class PointsController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ApplicationDbContext _context;

        public PointsController(IMemberService memberService, ApplicationDbContext context)
        {
            _memberService = memberService;
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPoints([FromBody] AddPointsDto addPointsDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _memberService.AddPointsAsync(addPointsDto.MemberId, addPointsDto.PurchaseAmount);
            if (!success)
            {
                return BadRequest(new { message = "Failed to add points. Member may not be verified or purchase amount is too low." });
            }

            return Ok(new { message = "Points added successfully." });
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberPoints(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(new { memberId = member.Id, totalPoints = member.TotalPoints });
        }
    }
}
