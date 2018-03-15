using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class CheckSessionFilter : IActionFilter
    {
        private ILogger<CheckSessionAttribute> _logger;

        public CheckSessionFilter(ILogger<CheckSessionAttribute> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var meterAttribute = context.Controller.GetType().GetTypeInfo()
                .GetCustomAttribute<CheckSessionAttribute>();

            if (meterAttribute == null)
            {
                return;
            }

            var session = context.HttpContext.Session;
            if (session.GetString("OrganisationName") == null)
            {
                context.Result = new RedirectToActionResult("SignIn", "Account", null);
                _logger.LogInformation("Session lost, redirecting to Sign In");
            }
            else
            {
                _logger.LogInformation("Session good");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context){}
    }
}