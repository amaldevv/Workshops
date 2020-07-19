using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Web.Authorization
{
    public class RoleAuthorizationRequirement : AuthorizationHandler<RoleAuthorizationRequirement>,  IAuthorizationRequirement
    {
        public RoleAuthorizationRequirement(IEnumerable<string> permissions)
        {
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public IEnumerable<string> Permissions { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RoleAuthorizationRequirement requirement)
        {
            var user = context.User;
            if (user == null || requirement.Permissions == null || !requirement.Permissions.Any())
                return Task.CompletedTask;

            var userName = user.FindFirst("LoginName");
            var hasPermission =
                requirement.Permissions.Any(permission => HasPermission(userName?.Value,permission));

            if (!hasPermission) return Task.CompletedTask;

            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private bool HasPermission(string username, string permission)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
            var permisions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"amal", "admin" },
                {"dev","employee" }
            };

            permisions.TryGetValue(username, out var found);
            return permission.Equals(found, StringComparison.OrdinalIgnoreCase);
        }
    }
}