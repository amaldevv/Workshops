### Using Identity Services for Authentication

# Step 1
Install `Microsoft.AspNetCore.Identity.EntityFrameworkCore;` in your project

# Step 2
Add Identity Services to the application in `ConfigureServices` method

```csharp
services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
```

Enable the Identity middleware by calling the `UseIdentity` in the `Configure` method
Make sure that you add it before calling `UseMvc`

```csharp
app.UseIdentity();
```
# Step 3
Create a class named `ApplicationUser` which is derived from `IdentityUser` class

```csharp


```