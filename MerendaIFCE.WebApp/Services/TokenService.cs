using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp
{
    public class TokenService
    {
        private readonly UserManager<IdentityUser> userManager;

        public string Issuer { get; set; } = "Almoco.Issuer";

        public string Audience { get; set; } = "Almoco.Audience";

        public SecurityKey SecurityKey { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TokenValidationParameters TokenValidationParameters { get; set; }

        public TokenService()
        {
            SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{FDA1690C-AE1D-426E-9B36-D58042C2DC68}"));
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = SecurityKey,
                ValidAudience = Audience,
                ValidIssuer = Issuer,

                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        public async Task<string> GetToken<T>(T user, UserManager<T> userManager) where T : IdentityUser
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
                Issuer = Issuer,
                Audience = Audience,
                Expires = now.AddMonths(1),
                IssuedAt = now,

                SigningCredentials = SigningCredentials,
                Subject = identity
            });

            var token = handler.WriteToken(securityToken);

            return token;
        }

    }
}
