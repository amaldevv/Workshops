using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApi.Filters
{
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var paramArgs = context.ActionArguments;

            foreach (var param in paramArgs)
            {


                if (param.Value == null)
                {
                    context.Result = new BadRequestObjectResult("Model is null");
                    return;
                }

                if (!context.ModelState.IsValid)
                {
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
