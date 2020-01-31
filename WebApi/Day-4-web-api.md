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
    var items = await Task.Run(() =>  empRepo.EmployeeList() );
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


    var employee = await Task.Run(() => empRepo.GetEmployeeDetails(id));
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
    await Task.Run(() => empRepo.AddEmployee(employee));
    
    return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
}
  ```
### Step 8
For updating an employee we will create the method which uses `HttpPut` verb and accepts id as the route parameter
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateEmployee(Employee employee)
{
    if (id != employee.Id)
    {
        return BadRequest();
    }
    await Task.Run(() => empRepo.SaveEmployee(employee));
    return NoContent();
}
  ```
### Step 9
Similarly for the delete operation, we will make use of the `HttpDelete` verb and pass the id value in route for passing the information

```csharp
[HttpDelete("{id}")]
public async Task<ActionResult<Employee>> DeleteEmployee(int id)
{
    var employee = await Task.Run(() => empRepo.GetEmployeeDetails(id));
    if (employee == null)
    {
        return NotFound();
    }

    empRepo.DeleteEmployee(id);
    

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


