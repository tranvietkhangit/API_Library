using Microsoft.AspNetCore.Identity;

namespace LapTrinhWeb2_API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
