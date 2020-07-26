## 1. Create a New Project

1. Create a new empty MVC core project
2. Select target framework as `netcoreapp31`
3. Add a new class library project for models called EmployeeManager.Models
4. Add a new model for Employee, create a class named Employee.cs and add properties

![Create new .NET Core Web Application](images/01-Create%20new%20project.PNG)

```csharp
public class Employee
{
    public string FirstName{ get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public int EmployeeId { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime DateOfBirth { get; set; }
}
```

5. Add a new model class for holding User details. Create a class file in the same project named User.cs and add the below properties

```csharp
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Passowrd { get; set; }
    public string Role { get; set; }

}
```

5. If you run the application now, you will see a web page with a message `Hello World`

## 2. Add Controller and Views

1. Create a new folder called Controllers and add a new controller called `EmployeeController`
2. Rename the action method Index to Employees

```csharp
public IActionResult Employees()
{
    return View();
}
```

3. We will now create views for employee management. Create a folder called Views, then add `_ViewImports.cshtml` & `_ViewStart.cshtml` files

4. In the _ViewImport.cshtml file add the below content

```csharp
@using EmployeeManager.Web

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

5. Comment out the contents in the _ViewStart.cshtml file for the tme being. We will revisit it at a later point

6. Add a new folder called Employee under the View folder and add a new view called `Employees.cshtml` and replace the existing content with the below content there

```html
@{
ViewData["Title"]="Employee List";
}
<h2>Employee List</h2>
```

If you run this app you will sill see the earlier content which we saw in step 1. For our newly created views to render we will need to do some bootstrapping

## 4. Configuring startup

1. Go to Startup.cs file, add the below line in the ConfigureServices method
` services.AddControllersWithViews();`

2. Replace the app.UseEndPoints call with the below one in Configure method

```csharp

app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default",  "{controller=Employee}/{action=Employees}");
            });
```

## 5. Add layout and views

1. Create a new folder called `wwwroot` under the root folder and place all your `js` and `css` files there. We will be using `jQuery` and `Bootstrap` for styling, so get that from the internet and place it under this folder.
2. Create a new folder called `Shared` under the View folder and add a `_Layout.cshtml` file
3. Replace the content in the layout file with the one given below. This will create a good looking UI with a header, a navigation menu, a content area and a footer

```html

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Employee Manager</title>

    <environment include="Development">
        <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.css" />
    </environment>
    <environment exclude="Development">
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"
              asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
              asp-fallback-test-class="sr-only" asp-fallback-test-property="position" asp-fallback-test-value="absolute"
              crossorigin="anonymous"
              integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" />
    </environment>
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-controller="Employee" asp-action="Index">
                    Employee Manager
                </a>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                    
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Employee" asp-action="Employees">Home</a>
                        </li>
                       
                    </ul>
                </div>

            </div>
        </nav>
    </header>
    <div class="container">
        
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2020 - Employee Manager - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>

    <environment include="Development">
        <script src="~/lib/jquery/dist/jquery.js"></script>
        <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.js"></script>
    </environment>
    <environment exclude="Development">
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"
                asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
                asp-fallback-test="window.jQuery"
                crossorigin="anonymous"
                integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=">
        </script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.bundle.min.js"
                asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"
                asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
                crossorigin="anonymous"
                integrity="sha384-xrRywqdh3PHs8keKZN+8zzc5TX0GRTLCcmivcbNJWm2rs5C8PRhcEn3czEjhAO9o">
        </script>
    </environment>
    <script src="~/js/site.js" asp-append-version="true"></script>

    @RenderSection("Scripts", required: false)
</body>
</html>

```

4. Modify the `Startup.cs` file to call the static files middleware so that all our css and js will be served to the browser. Add the below line in the configure method after the `app.UseRouting` statement

```csharp
app.UseStaticFiles();
```

5. Hit F5 to run the application and see it in full glow


## 6. Publishing to Azure

Let's see how we can publish it to Azure right from Visual Studio.

1. Right click your web project and select Publish
2. Select Azure from the popup window and click next
3. In the next window, select Azure App Service for Windows
4. Since we are deploying it as a new app service, select that option from the bottom
5. In the select window, give a name for the app service, select subscription, create a new resource group and app service plan and click on create
6. Once the deployment is completed, click on the finish button.
7. You will be presented with a summary page, from where you can browse the newly created app service
8. click on the publish button in the summary page to start deploying the app in to azure

## 7. Add local authentication

1. Create a view model for our login view. Create a new folder called `ViewModel` under the root folder of the project. We will create a base view model first and then inherit it in the view model for our login view. 
Create a new class called `BaseViewModel.cs` and add the below code

```csharp
public class BaseViewModel
{
    public List<string> Errors { get; set; }

    public BaseViewModel()
    {
        Errors = new List<string>();
    }
}

```

Add one more class named `LoginViewModel.cs`, add the below code

```csharp
public class LoginViewModel : BaseViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }

}

```

2. Let's start by creating a view for the login page. Create a new folder called Account under the views folder and create a view called `Login.cshtml`

3. Add the below code to create the UI

```html

@model EmployeeManager.Web.ViewModel.LoginViewModel
@{
    ViewData["Title"] = "Login";
}
<h1>Login</h1>
<form method="post">
    <div class="container">
        <div class="row">
            <div class="col"><label asp-for="Username"></label></div>
            <div class="col"><input asp-for="Username" /><span asp-validation-for="Username"></span></div>

        </div><div class="row">
            <div class="col">Last Name</div>
            <div class="col"><input asp-for="Password" type="password" /></div>
        </div>
        @if (Model != null && Model.Errors.Count > 0)
        {
            <div class="row">
                @foreach (var item in Model.Errors)
                {
                    <span>@item</span><br />
                }
            </div>
        }
        <div class="row">
            <div class="col"><button asp-controller="Account" asp-action="ValidateLogin">Login</button></div>
        </div>

    </div>
</form>
@section Scripts
{

    <script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
    <script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>

}

````

3. Add a new controller called `AccountController.cs` inside the `Controllers` folder and add the below code

```csharp
public IActionResult Login()
{
    return View();
}
```

4. Create action methods to validate login using the credentials entered in the UI as well as for logout

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


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
                        new Claim(ClaimTypes.Name, "amal")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), null);


                    
                    return  RedirectToAction("Employees", "Employee");

                }
                else
                    userdetails.Errors.Add("Incorrect login or password");
            }
            else
                userdetails.Errors.Add("Username/Password cannot be empty");

        }
    }
    return View("Login", userdetails);
}

public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Redirect("/");
}

```

5. Modify the `ConfigureServices` method to add cookie authentication method

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;


 services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
```

Make sure that `UseAuthentcation` middleware is called in this method, Modify the default route in the startup.cs to land in the login page instead of the Employees page

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseAuthentication();
    app.UseRouting();
    app.UseStaticFiles();
    app.UseEndpoints(endpoints =>
    {

        endpoints.MapControllerRoute("default","{controller=Account}/{action=Login}");
    });
}
```

6. Add a welcome link in the header. Create a partial view named _LoginPartial.cshtml and add it under the Shared folder under Views
Add the below code to create the links

```html

<ul class="navbar-nav">
    @if (Context.User.Identity.IsAuthenticated)
    {
        <li class="nav-item">
            <span class="nav-link text-dark">Hello @User.Identity.Name!</span>
        </li>
        <li class="nav-item">

            <form class="form-inline" asp-controller="Account" asp-action="Logout" asp-route-returnUrl="@Url.Action("Account", "Login")">
                <button type="submit" class="nav-link btn btn-link text-dark">Logout</button>
            </form>

        </li>
    }
    else
    {

        <li class="nav-item">
            <a class="nav-link text-dark" asp-page="/Account/Login">Login</a>
        </li>
    }
</ul>


```


7. Modify the _Layout.cshtml to call the login partial view

```html
<partial name="_LoginPartial" />
```

## 8. Implement Authorization

1. We will create a new policy for doing the authorization part in our application. Create a new folder called `Authorization` and add a new file called 
`RoleAuthorizationRequirement` to define our policy requirement

```csharp
public class RoleAuthorizationRequirement : AuthorizationHandler<RoleAuthorizationRequirement>, IAuthorizationRequirement
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
                requirement.Permissions.Any(permission => HasPermission(userName?.Value, permission));

            if (!hasPermission) return Task.CompletedTask;

            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private bool HasPermission(string username, string permission)
        {
            //DUMMY CODE to vreify the claim
            //IN real world scenario one will need to validate against the data stored in a database table
            if (string.IsNullOrWhiteSpace(username))
                return false;
            var permisions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"amal", "admin" },
                {"dev","employee" },
                {"guest","guest" }
            };

            permisions.TryGetValue(username, out var found);
            return permission.Equals(found, StringComparison.OrdinalIgnoreCase);
        }
    }
```

2. Modify the `Configure` method in the Startup.cs class to call the authorization middleware

```csharp
services.AddAuthorization(auth =>
{
    auth.AddPolicy("EmployeeOnly", policy =>
        policy.Requirements.Add(new RoleAuthorizationRequirement(new List<string> { "Admin","Employee" })));
});
```

3. Add the authorization middleware to the request pipeline in the `ConfigureServices` method. Make sure that you add this only after you called the routing middleware

```csharp
 app.UseAuthorization();
```

4. Add a new view for showing the access denied page

```html
<h1>AccessDenied</h1>

<span> You doesn't have permissions to access the requested resource<br />
    Click <a asp-controller="Account" asp-action="Login">here</a> to go back to the login page</span>
```

5. Also, add an action method in the `AccountController` to redirect to the view we just added

```csharp
public  IActionResult AccessDenied(string returnurl) => View();
```

4. Decorate your `EmployeeController` with the `Authorize` attribute

```csharp
[Authorize(Policy = "EmployeeOnly")]
public class EmployeeController : Controller
```

## 9. Implementing Azue AD Authentication
1. Create an AD in Azure
2. Register your application in Azure AD. Make a note of the tenant and application id values. These are needed for connecting to the AD from your code

3. Create an appsettings.json file for storing the details needed for connecting to the AD. If the file exists add the following section into the configuration file

```json
"AzureAD": {
   "Instance": "https://login.microsoftonline.com/",
   "Domain": "TechRepository105.onmicrosoft.com",
   "TenantId": "bf4f7fe7-2f17-408b-bb64-0c93d0f55ff6",
   "ClientId": "b5ef6d9c-8f34-451b-af2d-de1c5257b6f1",
   "CallbackPath": "/signin-oidc",
   "SignedOutCallbackPath ": "/signout-oidc"
  }
```

4. Let's add one more key in the appsettings.json file to determine whether we need to do auth with Azure AD. If the value is set to true then we will use Azure AD to validate the user otherwise we will the logic which we built up earlier
```json
"UseAzureAD": true
```
5. Install the nuget package `Microsoft.AspNetCore.Authentication.AzureADB2C.UI` in your web project

```bash
Install-Package Microsoft.AspNetCore.Authentication.AzureADB2C.UI
```


6. Modify the Program.cs file to hookup our configurations as shown below
```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            var root = config.Build();

            IHostEnvironment env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange:true);

        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}
```
7. Create a new constructor in the Startup.cs class which will inject the configuration class which can then be used to access the values in the json

```csharp
using Microsoft.Extensions.Configuration;

public IConfiguration Configuration { get; }
bool UseAzureAD;


public Startup(IConfiguration configuration)
{
    Configuration = configuration;
        UseAzureAD = Configuration.GetValue<bool>("UseAzureAD");
}
```

8. Modify the Configure method and add the below code to set up Azure AD authentication
```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;

public void ConfigureServices(IServiceCollection services)
        {
            if (UseAzureAD)
            {
                services.AddAuthentication(AzureADB2CDefaults.OpenIdScheme)
                    .AddAzureADB2C(opt => Configuration.Bind("AzureAD", opt))
                    .AddCookie();
                
            }
            else
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                   {
                       options.LoginPath = "/Account/Login";
                       options.LogoutPath = "/Account/Logout";
                   });

                services.AddControllersWithViews();
            }
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("EmployeeOnly", policy =>
                    policy.Requirements.Add(new RoleAuthorizationRequirement(new List<string> { "Admin","Employee" })));
            });

            
        }
```

9. Now we will need to show the Azure Ad login page when the app is started instead of the login page we created. So, let's modify the `AccountController` to add a constructor and implement the login and logout logic

```csharp
using Microsoft.Extensions.Configuration;

private IConfiguration Configuration { get; }
internal bool useAzureAD;

public AccountController(IConfiguration configuration)
{
    Configuration = configuration;
    useAzureAD = Configuration.GetValue<bool>("UseAzureAD");
}
```

10. And the login action method will be modified as below
```csharp
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;

public async Task<IActionResult> Login()
{

    if (useAzureAD)
    {
        await HttpContext.ChallengeAsync(AzureADB2CDefaults.OpenIdScheme,
        new AuthenticationProperties { RedirectUri = "/Employee/Employees" });

        // return  RedirectToAction("Employees", "Employee");
    }

    return View();
}

```

11. Logout method as follows

```csharp
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    if(useAzureAD)
    {
        var callbackUrl = Url.Action(nameof(Login), "Account", values: null, protocol: Request.Scheme);
        return SignOut(
            new AuthenticationProperties { RedirectUri = callbackUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            AzureADB2CDefaults.OpenIdScheme);
    }

    return Redirect("/");
}
```

12 . We will override open id events to add our own claims to the identity object. Create a new class called `ExtendedOpenIdConnectEvents.cs` inside the authorization folder

```csharp
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

public class ExtendedOpenIdConnectEvents : OpenIdConnectEvents
{
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
        
    }
}

```

13. Modify our startup.cs file to add this to authorization middleware

```csharp
    services.AddScoped<ExtendedOpenIdConnectEvents>();
    
    services.AddAuthentication(AzureADB2CDefaults.OpenIdScheme)
        .AddAzureADB2C(opt => Configuration.Bind("AzureAD", opt))
        .AddCookie();
    services.Configure<OpenIdConnectOptions>(AzureADB2CDefaults.OpenIdScheme, options =>
    {
        options.EventsType = typeof(ExtendedOpenIdConnectEvents);
    });

```

14. Let's publish this to our web app in Azure and see how it's working there.


[01-CreateNew]: (images/01-Create new project.PNG "")
