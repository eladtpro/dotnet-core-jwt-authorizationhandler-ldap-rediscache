using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS.Poc.Server.Core;
using MS.Poc.Server.Model;

namespace MS.Poc.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository users;
        private readonly ITokenRepository tokens;
        public LoginController(IUserRepository userRepo, ITokenRepository tokenRepo)
        {
            users = userRepo;
            tokens = tokenRepo;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<AuthenticationResponse> Authenticate([FromBody]AuthenticationRequest request)
        {
            // TODO: handle login errors
            IUser user = await users.Authenticate(request);
            JwtSecurityToken token = tokens.GetToken(user);
            //await cache.SetAsync<IUser>(user.Username, user, new DistributedCacheEntryOptions { AbsoluteExpiration = token.ValidTo });
            return new AuthenticationResponse(user, tokens.GetTokenString(token));
        }
    }
}
