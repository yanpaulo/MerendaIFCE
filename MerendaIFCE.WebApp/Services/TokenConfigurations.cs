using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Services
{
    public class TokenConfigurations
    {
        public string Issuer { get; set; } = "MerendaIFCE.Issuer";

        public string Audience { get; set; } = "MerendaIFCE.Audience";

        public SecurityKey SecurityKey { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TokenValidationParameters TokenValidationParameters { get; set; }


        public TokenConfigurations()
        {
            SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("C1EF72ED-164A-4EBC-8170-6FA1861D518B"));
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

    }
}
