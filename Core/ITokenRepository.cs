using MS.Poc.Server.Model;
using System.IdentityModel.Tokens.Jwt;

namespace MS.Poc.Server.Core
{
    public interface ITokenRepository
    {
        JwtSecurityToken GetToken(IUser user);
        string GetTokenString(JwtSecurityToken token);
    }
}
