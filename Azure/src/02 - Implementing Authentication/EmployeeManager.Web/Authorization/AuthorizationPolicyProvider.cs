using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Web.Authorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {

        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {

            if (!policyName.StartsWith("Role", StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPolicyAsync(policyName);
            }

            var permissionNames = policyName.Substring("Role".Length).Split(',');

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RoleAuthorizationRequirement(permissionNames))
                .Build();

            return Task.FromResult(policy);
        }
    }
}