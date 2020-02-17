using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EmployeeApi.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured, {ex.Message}, {typeof(NotSupportedException)}");
                //Logging logic goes here
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var message = String.Empty;
            var exceptionType = exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Access to the Web API is not authorized.";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotSupportedException))
            {
                message = "Not Supported.";
                context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            }
            else if (exceptionType == typeof(Exception))
            {
                message = "Internal Server Error.";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                message = "Not found.";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            

            await context.Response.WriteAsync($"{context.Response.StatusCode} - {message}");
        }
    }
}
