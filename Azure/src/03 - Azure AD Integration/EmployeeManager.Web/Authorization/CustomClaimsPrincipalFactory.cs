using EmployeeManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManager.Web.Authorization
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
    {
        public CustomClaimsPrincipalFactory(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);

            // Add your claims here
            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                new Claim("LoginName", user.Username) });

            return principal;
        }

    }
}