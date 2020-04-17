using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class CheckSessionFilter : IActionFilter
    {
        private ILogger<CheckSessionAttribute> _logger;
        private readonly ISessionService _sessionService;

        public CheckSessionFilter(ILogger<CheckSessionAttribute> logger, ISessionService sessionService)
        {
            _logger = logger;
            _sessionService = sessionService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            CheckSessionAttribute checkSessionAttribute = null;

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                checkSessionAttribute = controllerActionDescriptor
                    .MethodInfo
                    .GetCustomAttribute<CheckSessionAttribute>();
            }

            if(checkSessionAttribute == null)
            {
                checkSessionAttribute = context.Controller.GetType()
                    .GetTypeInfo()
                    .GetCustomAttribute<CheckSessionAttribute>();
            }

            if (checkSessionAttribute == null || checkSessionAttribute.CheckSession == CheckSession.Ignore)
            {
                return;
            }

            if (_sessionService.Get(checkSessionAttribute.Key) == null)
            {
                if (checkSessionAttribute.CheckSession == CheckSession.Error)
                {
                    context.Result = 
                        new BadRequestObjectResult("Session lost");

                    _logger.LogInformation($"Session lost, error result");
                }
                else if (checkSessionAttribute.CheckSession == CheckSession.Redirect)
                {

                    context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = checkSessionAttribute.Controller,
                            action = checkSessionAttribute.Action
                        }));

                    _logger.LogInformation($"Session lost, redirecting to {checkSessionAttribute.Action}");
                }
            }
            else
            {
                _logger.LogInformation("Session good");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context){}
    }
}