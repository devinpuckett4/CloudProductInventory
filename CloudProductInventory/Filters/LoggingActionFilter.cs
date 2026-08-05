using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudProductInventory.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> logger;

        public LoggingActionFilter(
            ILogger<LoggingActionFilter> logger)
        {
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            string controllerName =
                context.Controller.GetType().Name;

            string actionName =
                context.ActionDescriptor.RouteValues["action"]
                ?? "UnknownAction";

            string httpMethod =
                context.HttpContext.Request.Method;

            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName} | HTTP {HttpMethod}",
                DateTime.UtcNow.ToString("O"),
                controllerName,
                actionName,
                httpMethod);

            try
            {
                ActionExecutedContext resultContext =
                    await next();

                if (resultContext.Exception != null &&
                    !resultContext.ExceptionHandled)
                {
                    logger.LogError(
                        resultContext.Exception,
                        "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | {ErrorMessage}",
                        DateTime.UtcNow.ToString("O"),
                        controllerName,
                        actionName,
                        resultContext.Exception.Message);
                }
                else
                {
                    logger.LogInformation(
                        "{Timestamp} | EXIT | {ClassName}.{MethodName} | HTTP {HttpMethod}",
                        DateTime.UtcNow.ToString("O"),
                        controllerName,
                        actionName,
                        httpMethod);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    controllerName,
                    actionName,
                    ex.Message);

                throw;
            }
        }
    }
}