using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Models;
using EmployeeManager.Web.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
namespace EmployeeManager.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        bool UseAzureAD;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
             UseAzureAD = Configuration.GetValue<bool>("UseAzureAD");
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            //services.AddIdentity<User, IdentityRole>()

            // .AddDefaultTokenProviders();
            //    .AddDefaultTokenProviders();

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("AdminOnly", policy => policy.RequireClaim("HasAdminRole"));
            });

            if (UseAzureAD)
            {
                services.AddScoped<ExtendedOpenIdConnectEvents>();
                //services.AddMicrosoftWebAppAuthentication(Configuration);
                services.AddAuthentication(AzureADB2CDefaults.OpenIdScheme)
                    .AddAzureADB2C(opt => Configuration.Bind("AzureAD", opt))
                    .AddCookie();
                services.Configure<OpenIdConnectOptions>(AzureADB2CDefaults.OpenIdScheme, options =>
                 {
                     options.EventsType = typeof(ExtendedOpenIdConnectEvents);
                 });

            }
            else
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "/Account/Logout";
                    });
            }

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            
            //services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();

          
            if (UseAzureAD)
            {
                services.AddControllersWithViews(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }).AddMicrosoftIdentityUI();
            }
            else
                services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
                
               // .UseCookiePolicy();
            
            
            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllerRoute("default", UseAzureAD? "{controller=Account}/{action=Login}": "{controller=Account}/{action=Login}");
            });
        
        }
    }
}