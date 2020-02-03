using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcSampleDemo.Filters;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcSampleDemo.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository empRepo;
        private readonly ILogger<EmployeeController> logger;
        private readonly IConfiguration config;
        private string BaseUrl;
        JsonSerializerOptions jsonOptions;
        public EmployeeController(IEmployeeRepository EmpRepo, ILogger<EmployeeController> Logger, IConfiguration Config)
        {
            empRepo = EmpRepo;
            logger = Logger;
            config = Config;

            BaseUrl = config["API:BaseUrl"];

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

        }



        public async Task<IActionResult> Employees()
        {
            logger.LogInformation($"Inside Employee Controller -> {nameof(Employees)}");
            var employeeList = new List<Employee>();
            
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{BaseUrl}/api/Employee"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    employeeList = JsonSerializer.Deserialize<List<Employee>>(apiResponse, jsonOptions);
                }
            }
            return View(employeeList);
        }

        public IActionResult CreateEmployee()
        {
            logger.LogWarning($"Inside Employee Controller -> {nameof(CreateEmployee)}");
            return View();
        }
        [HttpPost]
        [ModelValidationFilter]
        public  async Task<IActionResult> SaveEmployee(Employee employee)
        {
            

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonSerializer.Serialize(employee, jsonOptions), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync($"{BaseUrl}/api/Employee",content))
                {
                    
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    
                }
            }
            //empRepo.AddEmployee(employee);
            return RedirectToAction("Employees");
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int Id)
        {
            var employee = new Employee();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{BaseUrl}/api/Employee/{Id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    employee = JsonSerializer.Deserialize<Employee>(apiResponse, jsonOptions);
                }
            }
            return View(employee);
            //return View(empRepo.GetEmployeeDetails(Id));
        }

        
        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
           
            if (employee != null)
            {
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
               
            }
            return Json(new { isSucess = false });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int Id)
        {
            if (Id > 0)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.DeleteAsync($"{BaseUrl}/api/Employee/{Id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return Json(new { isSucess = true });
                    }
                }
               

            }
            return Json(new { isSucess = false });
        }
    }
}

