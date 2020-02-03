using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MvcSampleDemo.Controllers;
using MvcSampleDemo.Models;
using MvcSampleDemo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MvcSampleDemo.Test
{
    public class ControllerUnitTest
    {
        Mock<IEmployeeRepository> mockEmpRepo;
        Mock<IConfiguration> mockConfig;
        Mock<ILogger<EmployeeController>> mockLogger;
        [Fact]
        public void Employees_ValidTest()
        {
            Initialize();
            mockEmpRepo.Setup(repo => repo.EmployeeList()).Returns(Task.FromResult(GetTestEmployees()));
            var controller = new EmployeeController(mockEmpRepo.Object, mockLogger.Object,mockConfig.Object);

            var result = controller.Employees();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Employee>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());

        }
        [Fact]
        public void  Employees_NoDataTest()
        {
            Initialize();
            mockEmpRepo.Setup(repo => repo.EmployeeList()).Returns(Task.FromResult(new List<Employee>()));
            var controller = new EmployeeController(mockEmpRepo.Object, mockLogger.Object, mockConfig.Object);

            var result = controller.Employees();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Employee>>(
                viewResult.ViewData.Model);
            Assert.Empty(model);

        }

        [Fact]
        public void CreateEmployee_ValidTest()
        {
            Initialize();
            var controller = new EmployeeController(mockEmpRepo.Object, mockLogger.Object, mockConfig.Object);

            // Act
            var result = controller.CreateEmployee();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); ;
        }

        [Fact]
        public void SaveEmployee_ValidTest()
        {
            Initialize();
            var controller = new EmployeeController(mockEmpRepo.Object, mockLogger.Object, mockConfig.Object);

            // Act
            var employee = new Employee { FirstName = "Test12", LastName = "RT1", EmailAddress = "Y@y.com" };
            var result = controller.SaveEmployee(employee);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Employees", redirectToActionResult.ActionName);
            mockEmpRepo.Verify();
        }

        private void Initialize()
        {
             mockEmpRepo = new Mock<IEmployeeRepository>();
             mockLogger = new Mock<ILogger<EmployeeController>>();
        }
        private List<Employee> GetTestEmployees()
        {
            var employees = new List<Employee>();
            employees.Add(new Employee() { Id = 1, FirstName = "Amal", LastName = "Dev", EmailAddress = "amal@gmail.com" });
            employees.Add(new Employee() { Id = 2, FirstName = "test", LastName = "user", EmailAddress = "test@gmail.com" });
            return employees;
        }
    }
}
