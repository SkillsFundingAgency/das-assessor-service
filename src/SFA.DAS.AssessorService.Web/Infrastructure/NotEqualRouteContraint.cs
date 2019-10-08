using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class NotEqualRouteContraint : IRouteConstraint
    {
        private string _match = string.Empty;

        public NotEqualRouteContraint(string match)
        {
            _match = match;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return String.Compare(values[routeKey].ToString(), _match, true) != 0;
        }
    }
}
