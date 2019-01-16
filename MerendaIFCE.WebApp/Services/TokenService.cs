using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MerendaIFCE.WebApp.Services
{
    public class TokenService<T> where T : IdentityUser
    {
        private readonly UserManager<T> userManager;
        private readonly TokenConfigurations tokenConfigurations;

        public TokenService(TokenConfigurations tokenConfigurations, UserManager<T> userManager)
        {
            this.tokenConfigurations = tokenConfigurations;
            this.userManager = userManager;
        }

        public async Task<string> GetToken(T user)
        {
            var now = DateTime.UtcNow;
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.UserName), claims);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                Expires = now.AddMonths(1),
                IssuedAt = now,
                SigningCredentials = tokenConfigurations.SigningCredentials,
                Subject = identity
            });

            var token = handler.WriteToken(securityToken);

            return token;
        }

    }
}
