# Implementing Logging in a .NET Core Applicaton using Serilog

## Step 1 : Setting up default logging

Create a new MVC application from Visual Studio
By default, Logging is enabled in the application via the ILogger interface. It has got some built in providers from writing the log information to console, event log as well as for third-party providers such as NLog, Serilog etc.

for example, if you want to write log file to the console window or event log, you will need to configure it in the `CreateDefaultHostBuilder` method in the `Program.cs` file

```csharp
 public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
        logging.AddEventLog();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });
```

In the case of web app, you will be get an `ILogger` from DI container and use that object for writing into the configured log providers
Let's see how we can do that in `HomeController`

First, create a private variable

```csharp
private readonly ILogger<HomeController> _logger;
```

Then modify to constructor as shown below

```csharp
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger;
}
```

Now, use the `LogInformation` method to write a message into the log

```csharp
public IActionResult Index()
{
    _logger.LogInformation("Writing to log");
    return View();
}
```

If run the application and goto the home page, you will see this message written to you console.

To write a error, we normally use `LogError` method as show below

```csharp
public IActionResult Index()
{
    _logger.LogInformation("Writing to log");
    _logger.LogError("Error from Serlog sample");
    return View();
}
```

And the output will be

### Step 2: Implementing Serilog to log in a file

First, you will need to install the necessary packages

```bash
Install-Package Serilog
Install-Package Serilog.Extensions.Logging
Install-Package Serilog.Sinks.File
```

Modify the `appsettings.json` file to include the path for the log file

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

Then modify the `Program.cs` file to read these values from the json file

```csharp
var configSettings = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.Build();

.ConfigureAppConfiguration(config =>
{
    config.AddConfiguration(configSettings);
})
```

Now, to hook up Serilog provider, import the namespace first

```csharp
using Serilog
```

An add the follwoing in the `CreateHostBuilder` method

```csharp
Log.Logger = new LoggerConfiguration()

    .WriteTo.File(configSettings["Logging:LogPath"])
    .CreateLogger();

.ConfigureLogging(logging =>
{
    logging.AddSerilog();
})
```

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var configSettings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    Log.Logger = new LoggerConfiguration()

        .WriteTo.File(configSettings["Logging:LogPath"])
        .CreateLogger();

    return Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
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

}
```

No run the application and you will see the information being written to the file mentoned in the path
