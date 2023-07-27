using Microsoft.AspNetCore.Routing;
using System;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeAuthorizationDeniedContext
    {
        public Guid PrivilegeId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}