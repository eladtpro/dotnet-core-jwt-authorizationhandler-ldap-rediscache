using Microsoft.AspNetCore.Authorization;

namespace MS.Poc.Server.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public const string Admin = "CN=Administrators,CN=Builtin,DC=mtkrn,DC=local";
        public const string User = "CN=Account Operators,CN=Builtin,DC=mtkrn,DC=local";

        public RoleRequirement(string role)
        {
            Role = role;
        }

        public string Role { get; }
    }
}
