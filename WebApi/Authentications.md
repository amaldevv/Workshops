# Role Based Authorization

### Step 1
Decorate your api methods with Authorize attributes

- GET Method

  ```[Authorize(Roles ="Read,Exclusive")]```

- GET method for employee details

   ```[Authorize(Roles = "Read,Exclusive")]```

- POST method
   
   ```[Authorize(Roles = "Exclusive,Add")]```

- PUT Method

   ```[Authorize(Roles = "Exclusive,Update")]```

- DELETE Method

   ```[Authorize(Roles = "Exclusive,Delete")]```

### Step 2
Configure Bearer Authentication in `ConfigureServices` method inside `Startup.cs`

```csharp
services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
```

Make sure that you enable the two middlewares in the `Configure` method in the same file

```csharp
app.UseAuthentication();
app.UseAuthorization();
```
### Step 3
Test it using the following bearer tokens

- Only Read
JWT Payload
```json
{
  "unique_name": "amal",
  "email": "amal@domain.com",
  "nbf": 1580495400,
  "exp": 1593541800,
  "iat": 1580495400,
  "role": [
    "Read"
  ]
}
```

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFtYWwiLCJlbWFpbCI6ImFtYWxAZG9tYWluLmNvbSIsIm5iZiI6MTU4MDQ5NTQwMCwiZXhwIjoxNTkzNTQxODAwLCJpYXQiOjE1ODA0OTU0MDAsInJvbGUiOlsiUmVhZCJdfQ.NDt3rexUWbAs2U3y_dtVxakboKl3NdFEcAm-IE_Jqb
```

- Read and Add
JWT Payload

```json
{
  "unique_name": "amal",
  "email": "amal@domain.com",
  "nbf": 1580495400,
  "exp": 1593541800,
  "iat": 1580495400,
  "role": [
    "Read", "Add"
  ]
}
```

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFtYWwiLCJlbWFpbCI6ImFtYWxAZG9tYWluLmNvbSIsIm5iZiI6MTU4MDQ5NTQwMCwiZXhwIjoxNTkzNTQxODAwLCJpYXQiOjE1ODA0OTU0MDAsInJvbGUiOlsiUmVhZCIsIkFkZCJdfQ.98WBVh1jTufrys1L2pfFewZ9kVBo8v_a4P8vEHKaDsQ
```
To test it using Postman, go to the Authorization tab, select Bearer as the option and set the above value inside token

# Custom Authorization Policies

### Step 1

Create  a requirement class inside the Filters folder named `PermissionRequirement` with a property and a constructor

```csharp
public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; set; }

    public PermissionRequirement(string permssionName) => PermissionName = permssionName; 
}
```

### Step 2 
Create a handler class for validting this requirement. Go to the Filters folder and create a class named `CustomAuthorizationHandler.cs` by inheriting the AuthorizationHandler class and override the 
`HandleRequirementAsync` method

```csharp
public class CustomAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
```

```csharp
protected override  Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
{
    if (requirement.PermissionName.Equals("Read") && IsUser     (context.User, context.Resource))
        context.Succeed(requirement);
    else if ((requirement.PermissionName.Equals("Read") || requirement.PermissionName.Equals("Delete")
        || requirement.PermissionName.Equals("Update") || requirement.PermissionName.Equals("Add") || requirement.PermissionName.Equals("Exclusive"))
        && IsOwner(context.User, context.Resource))
        context.Succeed(requirement);
    else
        context.Fail();

    return Task.CompletedTask;
}
```
Add the following two methods for validating the business logic


```csharp
private bool IsOwner(ClaimsPrincipal user, object resource) => user.Identity.Name.Equals("amal")?true:false;

private bool IsUser(ClaimsPrincipal user, object resource) => (user.Identity.Name.Equals("amal") || user.Identity.Name.Equals("dev") )? true : false;
```

### Step 3

Set up Bearer authentication and authorization policies inside the `ConfigureServices` method inside the `Startup.cs` class

```csharp
 services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

services.AddAuthorization(opt =>
{
    opt.AddPolicy("Read", policyOpt => policyOpt.AddRequirements (new PermissionRequirement("Read")));
    opt.AddPolicy("Exclusive", policyOpt => policyOpt.AddRequirements(new PermissionRequirement("Exclusive")));
    opt.AddPolicy("Add", policyOpt => policyOpt.AddRequirements(new PermissionRequirement("Add")));
    opt.AddPolicy("Delete", policyOpt => policyOpt.AddRequirements(new PermissionRequirement("Delete")));
    opt.AddPolicy("Update", policyOpt => policyOpt.AddRequirements(new PermissionRequirement("Update")));
});
```

### Step 4
Set up DI for our handler in the `ConfigureServices` method
valid user

```csharp
services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
```
### Step 5 
Decorate your action methods in the `EmployeeController` with the following attributes
- Get

```csharp
[Authorize(Policy = "Read")]
[Authorize(Policy = "Exclusive")]
```
- Create

```csharp
[Authorize(Policy = "Add")]
[Authorize(Policy = "Exclusive")]
```
- Update

```csharp
[Authorize(Policy = "Update")]
[Authorize(Policy = "Exclusive")]
```
- Delete

```csharp
[Authorize(Policy = "Delete")]
[Authorize(Policy = "Exclusive")]
```
### Step 6

To test it using Postman, go to the Authorization tab, select Bearer as the option and set the above value inside toke

**Valid User**

```json
{
  "unique_name": "amal",
  "email": "amal@domain.com",
  "nbf": 1582059526,
  "exp": 1593541800,
  "iat": 1582059526,
  "Roles":["Read","Exclusive"]
}
```
```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFtYWwiLCJlbWFpbCI6ImFtYWxAZG9tYWluLmNvbSIsIm5iZiI6MTU4MjA1OTUyNiwiZXhwIjoxNTkzNTQxODAwLCJpYXQiOjE1ODIwNTk1MjYsIlJvbGVzIjpbIlJlYWQiLCJFeGNsdXNpdmUiXX0.9823G5VqmqwW5b4UKW_rea-S0Ymnuj62sbo0P92nOQw
```

**Invalid user**

Payload
```json
{
  "unique_name": "dqev",
  "email": "dqev@domain.com",
  "nbf": 1582059526   ,
  "exp": 1593541800   ,
  "iat": 1582059526   ,
  "Roles":["Read"]
}
```

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImRxZXYiLCJlbWFpbCI6ImRxZXZAZG9tYWluLmNvbSIsIm5iZiI6MTU4MjA1OTUyNiwiZXhwIjoxNTgyMDU5NTU1LCJpYXQiOjE1ODIwNTk1MjYsIlJvbGVzIjpbIlJlYWQiXX0.WIzix1BfVLsFqaBDHS0LBST734kkti1hW_44EX2gWMM
```
