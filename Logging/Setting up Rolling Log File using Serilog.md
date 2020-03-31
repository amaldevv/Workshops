# Rolling Log File using Serilog

We all implement logging in whatever applications we develop and over time it will grow bigger by each passing day. If we don't control that over time we will run into problems, especially with the size. Most of the logging providers help to overcome this by using rolling log providers which automatically archives the current log file when it reaches specific criteria or a threshold and creates the new file to resume the logging. In this article we will how we can make of the rolling file provider supported by Serilog to implement this functionality.

## Step 1: Install Packages

First, install the following packages

```bash
Install-Package Serilog
Install-Package Serilog.Extensions.Logging
Install-Package Serilog.Sinks.File
```

The first package has all the core functionalities of Serilog whereas the second one is a provider for the logging subsystem used by ASP.NET Core(Microsoft.Extensions.Logging). The third package is responsible for writing the log information to the file, manages the rollover and all the related functionalities

## Step 2: Configure Serilog

Modify the `appsettings.json` file to include the path for the log file

```json
 "Logging": {
    "LogPath": "logs//ex.log",
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
```

Then, configure the sink to read this path from the config file and set it up to write to the file by modifying the `CreateHostBuilder` method in `Program.cs` file

```csharp
var configSettings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()

    .WriteTo.File(configSettings["Logging:LogPath"], rollOnFileSizeLimit:true,fileSizeLimitBytes:10)
    .CreateLogger();
```

Here, I configured the policy to roll over the file when the size of the current log file reaches 100 Kb. And finally, add the provider while bootstrapping the host as shown below

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var configSettings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    Log.Logger = new LoggerConfiguration()

        .WriteTo.File(configSettings["Logging:LogPath"], rollOnFileSizeLimit:true,fileSizeLimitBytes:100000)
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

## Step 3: Writing the log to the file

For a web application, use an object of `ILogger` which can be retrieved from the DI container as shown below

```csharp
//declare a private variable
private readonly ILogger<HomeController> _logger;

//assign the object got from the DI container in the constructor
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger;
}
```

if you run the application now and access the home page you will see the information is writing into the log file and rollover is happening automatically. The provider will automatically archive the file by appending a running sequence number to the file name.

```bash
ex.log
ex_001.log
ex_002.log
ex_003.log
```

## Rolling Policies

### Log file per day

If you want to configure rollover for a period, say for a day or month, you will need to set up the interval as shown below

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(configSettings["Logging:LogPath"], rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

This setting will create a log file per day

### Limits

When you set up rolling interval periods, be it for size or period, there are some default values

- By default, the size of the file is capped at 1 GB, so if are not limiting the file size it will grow up to 1 GB
`.WriteTo.File(configSettings["Logging:LogPath"], rollingInterval: RollingInterval.Day, fileSizeLimitBytes:100000)`
- Only the most recent 31 files are retained by default, you can override it by using `retainedFileCountLimit1`
`.WriteTo.File(configSettings["Logging:LogPath"], rollingInterval: RollingInterval.Day, retainedFileCountLimit: 100)`
