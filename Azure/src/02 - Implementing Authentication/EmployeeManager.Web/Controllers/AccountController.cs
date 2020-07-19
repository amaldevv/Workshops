using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManager.Models;
using EmployeeManager.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.Web.Controllers
{

    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> ValidateLoginAsync(LoginViewModel userdetails)
        {
            if (ModelState.IsValid)
            {
                if (userdetails != null)
                {
                    if (!string.IsNullOrWhiteSpace(userdetails.Username) && !string.IsNullOrWhiteSpace(userdetails.Password))
                    {
                        if (userdetails.Username.Equals("amal") && userdetails.Password.Equals("amal"))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, "amal"),
                                new Claim("FullName", "Administrator"),
                                new Claim(ClaimTypes.Role, "admin"),
                                new Claim("LoginName","amal")
                            };

                            var claimsIdentity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                //AllowRefresh = <bool>,
                                // Refreshing the authentication session should be allowed.

                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                                // The time at which the authentication ticket expires. A 
                                // value set here overrides the ExpireTimeSpan option of 
                                // CookieAuthenticationOptions set with AddCookie.

                                //IsPersistent = true,
                                // Whether the authentication session is persisted across 
                                // multiple requests. When used with cookies, controls
                                // whether the cookie's lifetime is absolute (matching the
                                // lifetime of the authentication ticket) or session-based.

                                //IssuedUtc = <DateTimeOffset>,
                                // The time at which the authentication ticket was issued.

                                //RedirectUri = <string>
                                // The full path or absolute URI to be used as an http 
                                // redirect response value.
                            };

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);
                            return RedirectToAction("Employees", "Employee");
                           
                        }
                        else
                            userdetails.Errors.Add("Incorrect login or password");
                    }
                    else
                        userdetails.Errors.Add("Username/Password cannot be empty");

                }
            }
            return View("Login",userdetails);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }


}