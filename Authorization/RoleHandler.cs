using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MS.Poc.Server.Core;
using System.Threading.Tasks;

namespace MS.Poc.Server.Authorization
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly string issuer;
        private readonly IUserRepository users;

        public RoleHandler(IConfiguration configuration, IUserRepository userRepo)
        {
            issuer = configuration["Jwt:Issuer"];
            users = userRepo;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            await Task.Run(async () =>
            {
                string username = context.User.FindFirst(
                    c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && //JwtRegisteredClaimNames.Sub && 
                    c.Issuer == issuer).Value;
                IUser user = await users.Find(username);
                if (null == user)
                    context.Fail();
                else if (user.Roles.Contains(requirement.Role))
                    context.Succeed(requirement);
                else
                    context.Fail();

            });
        }
    }
}
