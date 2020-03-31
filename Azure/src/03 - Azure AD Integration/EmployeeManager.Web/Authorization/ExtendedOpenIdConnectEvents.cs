using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManager.Web.Authorization
{
    public class ExtendedOpenIdConnectEvents : OpenIdConnectEvents
    {
        
        //public override Task RedirectToIdentityProvider(RedirectContext context)
        //{
        //    var state = new Dictionary<string, string> { { "reauthenticate", "true" } };
        //     context.ChallengeAsync(new AuthenticationProperties
        //     {
        //         RedirectUri = context.Request.Path,
        //     });

        //    return Task.FromResult(0);
        //}
         
        public override Task TokenValidated(TokenValidatedContext context)
        {
            var user = context.Principal.FindFirst("preferred_username");
            var claims = new List<Claim>();
            if (user != null && !(String.IsNullOrWhiteSpace(user?.Value)))
            {
                if (user.Value.Equals("dev@techrepository105.onmicrosoft.com"))
                {
                    claims.AddRange(new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, "amal"),
                                new Claim("FullName", "Administrator"),
                                new Claim(ClaimTypes.Role, "admin"),
                                new Claim("LoginName","amal")
                            });
                }
            }
            if (claims.Count > 0)
            {
                var claimsIdentity = new ClaimsIdentity(claims);

                context.Principal.AddIdentity(claimsIdentity);
            }
            return Task.FromResult(0);
            //return base.TokenValidated(context);
        }
    }
}
