using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class User : ClaimsPrincipal //IdentityUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Passowrd { get; set; }

        public string Role { get; set; }

        

        /*public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            var identity =  new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, this.UserName));
            
            return identity;
        }*/
    }
}