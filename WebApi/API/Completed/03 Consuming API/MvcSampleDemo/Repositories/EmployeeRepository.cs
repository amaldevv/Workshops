using MvcSampleDemo.Data;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSampleDemo.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        //public static List<Employee> EmployeeStore = new List<Employee>();

        private readonly EmployeeContext empContext;
        public EmployeeRepository(EmployeeContext EmpContext)
        {
            empContext = EmpContext;
        }

        public async Task<List<Employee>> EmployeeList()
        {
            //return EmployeeStore;
            return await Task.FromResult(empContext.Employees.ToList());
        }

        public async Task<int> AddEmployee(Employee employee)
        {
            try
            {
                /*employee.Id = EmployeeStore.Count > 0 ? EmployeeStore.Max(x => x.Id) + 1 : 1;
            EmployeeStore.Add(employee);*/

                //add employee to the collection
                empContext.Employees.Add(employee);
                //persist data to DB
                return await Task.FromResult(empContext.SaveChanges());


            }
            catch (Exception)
            {

                return -1;
            }

        }

        public async Task<bool> DeleteEmployee(int employeeId)
        {
            var employee = empContext.Employees.Find(employeeId);
            empContext.Employees.Remove(employee);
            if (empContext.SaveChanges() > 0)
                return await Task.FromResult(true);
            return await Task.FromResult(false);
        }



        public async Task<Employee> GetEmployeeDetails(int Id)
        {
            /*return EmployeeStore.Where(x => x.Id.Equals(Id))
                               .FirstOrDefault();*/
            return await Task.FromResult(empContext.Employees.FirstOrDefault(x => x.Id.Equals(Id)));
        }

        public async Task<bool> SaveEmployee(Employee employee)
        {

            var emp = empContext.Employees.Find(employee.Id);
            if (emp != null)
            {
                emp.FirstName = employee.FirstName;
                emp.LastName = employee.LastName;
                emp.EmailAddress = employee.EmailAddress;
                empContext.Update(emp);
                if (empContext.SaveChanges() > 0)
                    return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}
