using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Web.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {

        public RoleAuthorizeAttribute(params string[] permissions)
        {
            
            Policy = $"Role{String.Join(",",permissions)}";
        }
    }
}