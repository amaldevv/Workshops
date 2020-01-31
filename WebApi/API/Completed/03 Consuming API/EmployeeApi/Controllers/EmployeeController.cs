using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var items = await Task.Run(() => empRepo.EmployeeList());
            if (items == null)
            {
                return NotFound();
            }

            return items;
        }

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

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            await Task.Run(() => empRepo.AddEmployee(employee));
            
            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            await Task.Run(() => empRepo.SaveEmployee(employee));
                
            return NoContent();


        }

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
    }
}