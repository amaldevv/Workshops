using MvcSampleDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSampleDemo.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> EmployeeList();
        Task<Employee> GetEmployeeDetails(int Id);
        Task<int> AddEmployee(Employee employee);
        Task<bool> SaveEmployee(Employee employee);
        Task<bool> DeleteEmployee(int employeeId);
    }
}
