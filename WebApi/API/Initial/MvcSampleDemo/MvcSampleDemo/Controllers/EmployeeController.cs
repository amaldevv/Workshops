using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcSampleDemo.Filters;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;
using System.Threading.Tasks;

namespace MvcSampleDemo.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository empRepo;
        private readonly ILogger<EmployeeController> logger;
        public EmployeeController(IEmployeeRepository EmpRepo, ILogger<EmployeeController> Logger)
        {
            empRepo = EmpRepo;
            logger = Logger;
        }



        public async Task<IActionResult> Employees()
        {
            logger.LogInformation($"Inside Employee Controller -> {nameof(Employees)}");
            return View(await empRepo.EmployeeList());
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
            
            await empRepo.AddEmployee(employee);
            return RedirectToAction("Employees");
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int Id)
        {
            return View(await empRepo.GetEmployeeDetails(Id));
        }

        
        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
           
            if (employee != null)
            {
                if (await empRepo.SaveEmployee(employee))
                {
                    return Json(new { isSucess = true });
                }
            }
            return Json(new { isSucess = false });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int Id)
        {
            if (Id > 0)
            {
                if (await empRepo.DeleteEmployee(Id))
                {
                    return Json(new { isSucess = true });
                }

            }
            return Json(new { isSucess = false });
        }
    }
}

