using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository empRepo;
        public EmployeeController(IEmployeeRepository EmpRepo)
        {
            empRepo = EmpRepo;
        }
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            var items = await empRepo.EmployeeList();
            if (items == null)
            {
                return NotFound();
            }

            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {


            var employee = await empRepo.GetEmployeeDetails(id);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpPost]
        [ModelValidationFilter]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            throw new NotSupportedException("An error occured");
            await empRepo.AddEmployee(employee);
            
            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        [ModelValidationFilter]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            await empRepo.SaveEmployee(employee);
                
            return NoContent();


        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await empRepo.GetEmployeeDetails(id);
            if (employee == null)
            {
                return NotFound();
            }

           await empRepo.DeleteEmployee(id);
            

            return employee;
        }
    }
}