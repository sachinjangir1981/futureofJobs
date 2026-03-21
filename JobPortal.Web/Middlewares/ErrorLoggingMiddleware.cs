namespace JobPortal.Web.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Serilog;
    using System;
    using System.Threading.Tasks;

    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Unhandled exception. Path: {Path}, User: {User}, Query: {Query}",
                    context.Request.Path,
                    context.User?.Identity?.Name,
                    context.Request.QueryString);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }

}
