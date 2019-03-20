using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MerendaIFCE.WebApp.Services
{
    public class AppPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly IHttpContextAccessor httpContext;

        public AppPolicyProvider(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            AuthorizationPolicy policy;
            if (httpContext.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
            }
            else
            {
                policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            }

            return Task.FromResult(policy);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            throw new NotImplementedException();
        }
    }
}
