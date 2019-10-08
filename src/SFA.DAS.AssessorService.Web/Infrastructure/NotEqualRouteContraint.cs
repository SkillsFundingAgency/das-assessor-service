using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class NotEqualRouteContraint : IRouteConstraint
    {
        private readonly string _match;

        public NotEqualRouteContraint(string match)
        {
            _match = match;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return string.Compare(values[routeKey].ToString(), _match, true) != 0;
        }
    }
}
