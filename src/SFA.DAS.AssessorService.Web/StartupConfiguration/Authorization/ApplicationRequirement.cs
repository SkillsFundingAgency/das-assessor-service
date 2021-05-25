using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class ApplicationRequirement : IAuthorizationRequirement
    {
        public ApplicationRequirement(string routeId)
        {
            RouteId = routeId;
        }

        public string RouteId { get; }
    }
}