﻿using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
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
            var checkSessionAttribute = context.Controller.GetType().GetTypeInfo()
                .GetCustomAttribute<CheckSessionAttribute>();

            if (checkSessionAttribute == null)
            {
                return;
            }

            if (_sessionService.Get("OrganisationName") == null)
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