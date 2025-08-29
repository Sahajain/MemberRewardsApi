using MemberRewards.Models;

namespace MemberRewards.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(Member member);
    }
}