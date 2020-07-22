using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MS.Poc.Server.Core;
using MS.Poc.Server.Extensions;
using MS.Poc.Server.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MS.Poc.Server.Repositories
{
    class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public static IDictionary<string, AuthorizationPolicy> Policies(IConfiguration config)
        {
            AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder();
            IList<string> roles = config.List<string>("Authorization:Roles");
            Dictionary<string, AuthorizationPolicy> policies = new Dictionary<string, AuthorizationPolicy>();
            foreach (string role in roles)
                policies[role] = builder.RequireAuthenticatedUser().RequireRole(role).Build();
            return policies;
        }

        public JwtSecurityToken GetToken(IUser user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            IList<Claim> claims = GetClaims(user);

            DateTime now = DateTime.Now;

            return new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                now,
                now.Add(TimeSpan.Parse(configuration["Jwt:Expiry"])),
                credentials);
        }
        public string GetTokenString(JwtSecurityToken token)
        {
            string tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenJson;
        }

        private static IList<Claim> GetClaims(IUser user)
        {
            // TODO: add userInfo endpoint - make claims as short and small as can be - only needed claims
            return new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                //new Claim(ClaimTypes.WindowsAccountName, user.Username),
                //new Claim(ClaimTypes.Email, user.Email),
                //new Claim(ClaimTypes.Name, user.DisplayName),
                //new Claim(ClaimTypes.Name, user.DisplayName),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };
        }
    }
}
