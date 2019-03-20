using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace MerendaIFCE.WebApp.Services
{
    public class AppPolicyProvider : IAuthorizationPolicyProvider
    {
        public const string SignalRPolicyName = "SignalRPolicy";

        private readonly IHttpContextAccessor httpContext;
        private readonly AuthorizationPolicy jwtPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
        private readonly AuthorizationPolicy cookiePolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme).RequireAuthenticatedUser().Build();

        public AppPolicyProvider(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            AuthorizationPolicy policy;
            if (httpContext.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                policy = jwtPolicy;
            }
            else
            {
                policy = cookiePolicy;
            }

            return Task.FromResult(policy);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            switch (policyName)
            {
                case SignalRPolicyName:
                    return Task.FromResult(jwtPolicy);
                default:
                    throw new InvalidOperationException($"Policy with name {policyName} does not exist");
            }
        }
    }
}
