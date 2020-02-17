using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcSampleDemo.Filters;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private string accessToken;
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

            accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3QgU2FtcGxlIiwiaWF0IjoxNTgxODQ0MDAxLCJleHAiOjE1ODI3MDgwMDF9.FPCEfjnSIvLvgQlD68ucXi5f9o5idlZ_MJkFD8V6Wac";

        }



        public async Task<IActionResult> Employees()
        {
            logger.LogInformation($"Inside Employee Controller -> {nameof(Employees)}");
            var employeeList = new List<Employee>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer", accessToken);
                
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
        public async Task<IActionResult> SaveEmployee(Employee employee)
        {


            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                   "Bearer", accessToken);


                StringContent content = new StringContent(JsonSerializer.Serialize(employee, jsonOptions), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync($"{BaseUrl}/api/Employee", content))
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
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                   "Bearer", accessToken);


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

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                   "Bearer", accessToken);


                    StringContent content = new StringContent(JsonSerializer.Serialize(employee, jsonOptions), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PutAsync($"{BaseUrl}/api/Employee/{employee.Id}", content))
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
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                   "Bearer", accessToken);

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

