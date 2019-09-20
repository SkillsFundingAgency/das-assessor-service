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
            CheckSessionAttribute checkSession = null;

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                checkSession = controllerActionDescriptor
                    .MethodInfo
                    .GetCustomAttribute<CheckSessionAttribute>();
            }

            if(checkSession == null)
            {
                checkSession = context.Controller.GetType()
                    .GetTypeInfo()
                    .GetCustomAttribute<CheckSessionAttribute>();
            }

            if (checkSession == null || checkSession.CheckSession == CheckSession.Ignore)
            {
                return;
            }

            if (_sessionService.Get(checkSession.Key) == null)
            {
                if (checkSession.CheckSession == CheckSession.Error)
                {
                    context.Result = 
                        new BadRequestObjectResult("Session lost");

                    _logger.LogInformation($"Session lost, error result");
                }
                else if (checkSession.CheckSession == CheckSession.Redirect)
                {

                    context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = checkSession.Controller,
                            action = checkSession.Action
                        }));

                    _logger.LogInformation($"Session lost, redirecting to {checkSession.Action}");
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