# Getting started with Web Api

### Step 1
- Create new web app project
- Select API template
- Set API project as start up, if multiple projects are there in the solution
- Run the app

### Step 2

Add the below connection string in appsettings.json
```json
"ConnectionStrings": {
    "EmployeeDBConnection": "Server=(localdb)\\mssqllocaldb;Database=Employee;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
  ```

  ### Step 3

  Modify the program.cs, to read the configuration from the config file

  ```csharp
 public static IHostBuilder CreateHostBuilder(string[] args)
  {
      var configSettings = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json")
      .Build();

      return Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration(config =>
          {
              config.AddConfiguration(configSettings);
          })
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.UseStartup<Startup>();
          });
  }
        
  ```
### Step 4

 Modify startup.cs file to set up DB context and DI for emp repo

 ```csharp
services.AddDbContext<EmployeeContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("EmployeeDBConnection")));

services.AddControllers();

services.AddScoped<IEmployeeRepository, EmployeeRepository>();
  ```
### Step 5

Create a new API controller called Employee Controller, create a local private variable to hold the instance of the Employee repository injected via the constructor

  ```csharp
private readonly IEmployeeRepository empRepo;

public EmployeeController(IEmployeeRepository EmpRepo)
{
    empRepo = EmpRepo;
}
```

### Step 6
Create a method for retrieving the employees, decorate it with `HttpGet` attribute. Consume the `EmployeeList()` method in the repository to get the list of employees. If nothing is found, returns a `NotFound` status code back.

```csharp
[HttpGet]
public async Task<ActionResult<List<Employee>>> GetEmployees()
{
    var items = await empRepo.EmployeeList() ;
    if (items == null)
    {
        return NotFound();
    }
    return items;
}
```
### Step 7
Create another method to get the details of an employee. Here also `HttpGet` will be used, but addtionally we will specify **attribute routing** here. This method will get executed if and only if the id parameter is supplied
  
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Employee>> GetEmployee(int id)
{


    var employee = empRepo.GetEmployeeDetails(id);
    if (employee == null)
    {
        return NotFound();
    }
    return employee;
}
```
### Step 8
Let's create a **POST** method for creating a new employee. Decorate the method with `HttpPost` and call the `CreateEmployee` method availabe in the repository class 
  ```csharp
[HttpPost]
public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
{
    await empRepo.AddEmployee(employee);
    
    return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
}
  ```
### Step 8
For updating an employee we will create the method which uses `HttpPut` verb and accepts id as the route parameter
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateEmployee(int id,Employee employee)
{
    if (id != employee.Id)
    {
        return BadRequest();
    }
    await  empRepo.SaveEmployee(employee);
    return NoContent();
}
  ```
### Step 9
Similarly for the delete operation, we will make use of the `HttpDelete` verb and pass the id value in route for passing the information

```csharp
[HttpDelete("{id}")]
public async Task<ActionResult<Employee>> DeleteEmployee(int id)
{
    var employee = empRepo.GetEmployeeDetails(id);
    if (employee == null)
    {
        return NotFound();
    }

    await empRepo.DeleteEmployee(id);
    

    return employee;
}
  ```

# Help Documentation

 Adding help pages using Swagger, also known as Open API
 Swagger UI offers a web based ui that provides information about a service
Two implementations  are there
Swashbuckle
NSwag

Swashbuckle consists of three components
Core, SwaggerGen and SwaggerUI
Add pacakge
```bash
Swashbuckle.AspNetCore
```
Add the swagger generator in service method

```csharp
// Register the Swagger generator, defining 1 or more Swagger documents
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    });
```

 In the configure method, enable middleware for serving json and swagger ui

 ```csharp
// Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    })
 ```

 run the app, help endpoint will be
 ```html
 https://localhost:<port>/swagger/index.html
 ```
  ```html
 https://localhost:<port>/swagger/v1/swagger.json
 ```
# Consuming the API

### Step 1
Add the entry in config file for the base url of the API

```json
"API": {
    "BaseUrl": "https://localhost:44388"
  },
```

and then add the following in `EmployeeController` 

```csharp
private string BaseUrl;

//add this inside constructor
BaseUrl = config["API:BaseUrl"];
```

### Step 2
Set up the default JSON Serializer
Import the `System.Text.Json` namespace

```csharp
using System.Text.Json;
```

Create an instance of `JsonSerializerOptions` class and set the options

```csharp
JsonSerializerOptions jsonOptions;

//inside constructor
jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
```

### Step 3
We will use the `HttpClient` class available in `System.Net.Http` namspace to communicate with the API

```csharp
using System.Net.Http;
```

### Step 4
Modify the `Employee` method with the following snippet to retrieve all the employees using API
```csharp
using (var httpClient = new HttpClient())
{
    using (var response = await httpClient.GetAsync($"{BaseUrl}/api/Employee"))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();
        employeeList = JsonSerializer.Deserialize<List<Employee>>(apiResponse, jsonOptions);
    }
}
```

### Step 5
Modify `SaveEmployee` method with below for saving 
```csharp
using (var httpClient = new HttpClient())
{
    StringContent content = new StringContent(JsonSerializer.Serialize(employee, jsonOptions), Encoding.UTF8, "application/json");
    using (var response = await httpClient.PostAsync($"{BaseUrl}/api/Employee",content))
    {
        
        string apiResponse = await response.Content.ReadAsStringAsync();
        
    }
}
```

### Step 6
For editing, first you need to retrieve the employee details using API, so modify the method with the one given below
```csharp
using (var httpClient = new HttpClient())
{
    using (var response = await httpClient.GetAsync($"{BaseUrl}/api/Employee/{Id}"))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();
        employee = JsonSerializer.Deserialize<Employee>(apiResponse, jsonOptions);
    }
}
```
And for updating the data

```csharp
using (var httpClient = new HttpClient())
{
    var content = new MultipartFormDataContent();
    content.Add(new StringContent(employee.Id.ToString()), "id");
    content.Add(new StringContent(employee.FirstName), "firstName");
    content.Add(new StringContent(employee.LastName), "lastName");
    content.Add(new StringContent(employee.EmailAddress), "emailAddress");

    using (var response = await httpClient.PutAsync($"{BaseUrl}/api/Employee", content))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();

        return Json(new { isSucess = true });
    }
}
```
### Step 7

And finally for delete modify the `DeleteEmployee` method
```csharp
using (var httpClient = new HttpClient())
{
    using (var response = await httpClient.DeleteAsync($"{BaseUrl}/api/Employee/{Id}"))
    {
        string apiResponse = await response.Content.ReadAsStringAsync();
        return Json(new { isSucess = true });
    }
}
```

# Model Valiadation
To perform a valiadtion against a model, we can use the `Isvalid` to verify all the requirements set by the data annotation attributes are passing or not

```csharp
if (!ModelState.IsValid)
{
    return Page();
}
```
## Implementing Model Validation filters
If you want to move these kind of validation checking to a central location, it's better to write that logic inside a filter. It's possible to create an Action filter by inherting the `ActionFilter` class. There are two methods available in that class to override
- OnActionExecuting 
- OnActionExecuted

The first method will get executed before each action method in the controller is executed and the second will get executed after the ones in your controller completes the execution

### Step 1
Add a new folder named **Filters** in your Web API project and add new class named **ModelValidationFilter** inside it

### Step 2
Import the `Microsoft.AspNetCore.Mvc.Filters` namspace

```csharp
using Microsoft.AspNetCore.Mvc.Filters;
```
Inherit `ActionFilterAttribute` class

```csharp
public class ModelValidationFilter : ActionFilterAttribute
```
### Step 4

Now, modify the `OnActionExecuting` method with the following snippet. This will verify whether the incoming model for that action method is a valid one and the control will be passed to our action method only if it passes the validation. If it fails, then it will return with an error back to the response stream

```csharp
public override void OnActionExecuting(ActionExecutingContext context)
{

    var param = context.ActionArguments.SingleOrDefault();

    if (param.Value == null)
    {
        context.Result = new BadRequestObjectResult("Model is null");
        return;
    }

    if (!context.ModelState.IsValid)
    {
        context.Result = new BadRequestObjectResult(context.ModelState);
    }
}
```
### Step 5

Decorate the action methods in your web api controller so that the filter will be used for validating the incoming payload

```csharp

[HttpPost]
[ModelValidationFilter]
public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)


[HttpPut("{id}")]
[ModelValidationFilter]
public async Task<IActionResult> UpdateEmployee(int id, Employee employee)

```
# Handling Exceptions

Whenever there is an exception happening in the Web API project you will get a developer exception page with details about the exception. By default this page will be available only in the developer environment and is normally returned as an html. But in the case of WEB API , the content is returned in plain text format. To get an HTML formatted response, set media type as `text/html` for the 'Accept` request header.

### Step 1

To show a meaningful error page in upper ennvironments, use the Exception Handling Middleware available in the framework. To invoke it, modify the `Configure` method in the `Startup` class with the below snippet. When an exception occurs in non-prod environments, the control will be rediected to the page mentioned while invoking the middleware. Here it will look for an action method `Index` in a  controller named `Error`. Make sure that you call this before any middlewares, so that the exceptions happened inside that can also be handled

```csharp
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

```
### Step 2

We don't have a controller named like that in our web api project right, So let's create one  and add attribute routing for the error method

```csharp
[ApiController]
public class ErrorController : ControllerBase
{
    [Route("/api/error")]
    public IActionResult Error() => Problem();
}
```
### Step 3 Custom Exception Middleware

Middlewares can be used to catch unhandled exceptions in your code. You can write your own logic inside that and add it to the pipeline and configure it to handle all the exceptions for you. Create a new folder named `Middlewares` and a class named `GlobalExceptionMiddleware`


```csharp
public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (NotSupportedException ex)
            {
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var message = String.Empty;
            var exceptionType = exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Access to the Web API is not authorized.";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotSupportedException))
            {
                message = "Not Supported.";
                context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            }
            else if (exceptionType == typeof(Exception))
            {
                message = "Internal Server Error.";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                message = "Not found.";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            

            await context.Response.WriteAsync($"{context.Response.StatusCode} - {message}");
        }
    }

```

## Implement File Logging with Serilog

### Step 1
Add the following packages
```bash
Install-Package Serilog
Install-Package Serilog.Extensions.Logging
Install-Package Serilog.Sinks.Console
Install-Package Serilog.Sinks.File
```
Modify ```appsettings.json``` to include log file path
```json
  "Logging": {
    "LogPath": "logs//ex.log",
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
```
In program.cs modify the ```CreateHostBuilder``` method
to add config settings

```csharp
 var configSettings = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
```
Import Serilog namespace and add logger config
```csharp
using Serilog;
```


```csharp
 Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(configSettings["Logging:LogPath"])
                .CreateLogger();
```               
and then configure app configuration and logging
```csharp
 return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configSettings);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```

Modify the middleware to implement the logger, by injecting the logger instance 

```csharp
 private readonly ILogger<GlobalExceptionMiddleware> _logger;

public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    _next = next;
    _logger = logger;
}
```

Then modify the `InvokeAsync` method to call the `LogError` method
```csharp
public async Task InvokeAsync(HttpContext httpContext)
{
    try
    {
        await _next(httpContext);
    }
    catch (UnauthorizedAccessException ex)
    {
        _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
        //Logging logic goes here
        await HandleExceptionAsync(httpContext, ex);
    }
    catch (NotSupportedException ex)
    {
        _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
        //Logging logic goes here
        await HandleExceptionAsync(httpContext, ex);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
        //Logging logic goes here
        await HandleExceptionAsync(httpContext, ex);
    }
}
```
### Unit Testing API

### Step 1 
Add new xUnit test project to the solution, name the project as EmployeeAPI.Tests

### Step 2
Refer the API project in your solution by right clicking on the test project, then Add Reference and select the project

### Step 3 
Add the following package to you project by executing the following commands in Package Manager Console

```bash
Install-Package Microsoft.AspNetCore.Mvc
Install-Package Microsoft.AspNetCore.Mvc.Core
Install-Package Microsoft.AspNetCore.Diagnostics
Install-Package Microsoft.AspNetCore.TestHost
Install-Package Microsoft.Extensions.Configuration.Json
```
### Step 4
Add a new class file to bootstrap our WebAPI, let's name that as `TestFixture.cs`

Add the following namespaces first
```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
```

```csharp
public class TestFixture<TStartup> : IDisposable
{
    public static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
    {
        var projectName = startupAssembly.GetName().Name;

        var applicationBasePath = AppContext.BaseDirectory;

        var directoryInfo = new DirectoryInfo(applicationBasePath);

        do
        {
            directoryInfo = directoryInfo.Parent;

            var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

            if (projectDirectoryInfo.Exists)
                if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj")).Exists)
                    return Path.Combine(projectDirectoryInfo.FullName, projectName);
        }
        while (directoryInfo.Parent != null);

        throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
    }

    private TestServer Server;

    public TestFixture()
        : this(Path.Combine(""))
    {
    }

    public HttpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
        Server.Dispose();
    }

    protected virtual void InitializeServices(IServiceCollection services)
    {
        var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

        var manager = new ApplicationPartManager
        {
            ApplicationParts =
            {
                new AssemblyPart(startupAssembly)
            },
            FeatureProviders =
            {
                new ControllerFeatureProvider(),
                new ViewComponentFeatureProvider()
            }
        };

        services.AddSingleton(manager);
    }

    protected TestFixture(string relativeTargetProjectParentDir)
    {
        var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
        var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json");

        var webHostBuilder = new WebHostBuilder()
            .UseContentRoot(contentRoot)
            .ConfigureServices(InitializeServices)
            .UseConfiguration(configurationBuilder.Build())
            .UseEnvironment("Development")
            .UseStartup(typeof(TStartup));

        // Create instance of test server
        Server = new TestServer(webHostBuilder);

        // Add configuration for client
        Client = Server.CreateClient();
        Client.BaseAddress = new Uri("https://localhost:44388");
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
```
### Step 6
Now we will create a new class for writing our test methods

```csharp
public class EmployeeControllerTests : IClassFixture<TestFixture<Startup>>
{
    private HttpClient Client;

    public EmployeeControllerTests(TestFixture<Startup> fixture)
    {
        Client = fixture.Client;
    }

    [Fact]
    public async Task EmployeeListGetTestAsync()
    {
        // Arrange
        var request = "/api/Employee";

        // Act
        var response = await Client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task EmployeeCreateTestAsync()
    {
        // Arrange
        var request = new
        {
            Url = "/api/Employee",
            Body = new
            {
                FirstName ="Api",
                LastName = "test",
                EmailAddress = "apitest@gmail.com"
            }
        };

        // Act
        var response = await Client.PostAsync(request.Url, GetStringContent(request.Body));
        var value = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
    }

    public static StringContent GetStringContent(object obj)
        => new StringContent(JsonSerializer.Serialize(obj), Encoding.Default, "application/json");
}
```

# Enable Cors

 Add the following 

 ```chsarp
 services.AddCors(policy =>
{
    policy.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
});
```

```csharp
app.UseCors("AllowOrigin");
```


