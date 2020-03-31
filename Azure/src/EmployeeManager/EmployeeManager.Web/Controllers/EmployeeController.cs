using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.Web.Controllers
{
    [RoleAuthorize("Admin")]
    public class EmployeeController : Controller
    {
        public IActionResult Employees()
        {
            return View();
        }
    }
}