using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS.Poc.Server.Authorization;
using MS.Poc.Server.Core;
using MS.Poc.Server.Model;
using System.Threading.Tasks;

namespace MS.Poc.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        //TODO: user info endpoint
        private readonly IUserRepository users;

        public UserController(IUserRepository userRepo)
        {
            users = userRepo;
        }

        [HttpGet]
        [Route("userdata")]
        [Authorize(Policy = RoleRequirement.User)]
        public async Task<string> UserData(string username)
        {
            IUser user = await users.Find(username);
            return user.DisplayName;
        }

        [HttpGet]
        [Route("admindata")]
        [Authorize(Policy = RoleRequirement.Admin)]
        public async Task<IUser> AdminData(string username)
        {
            IUser user = await users.Find(username);
            return user;
        }
    }
}
