using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeAuthorizationHandler : AuthorizationHandler<PrivilegeRequirement>
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataProvider _tempDataProvider;

        public PrivilegeAuthorizationHandler(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, ITempDataProvider tempDataProvider)
        {
            _contactsApiClient = contactsApiClient;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PrivilegeRequirement requirement)
        {
            if (!_httpContextAccessor.HttpContext.User.HasClaim(c => c.Type == "UserId"))
            {
                context.Fail();
                return;
            }
            
            var userid = _httpContextAccessor.HttpContext.User.FindFirst("UserId").Value;

            var privilegeRequested = (await _contactsApiClient.GetPrivileges()).FirstOrDefault(p => p.Key.Equals(requirement.Privilege, StringComparison.InvariantCultureIgnoreCase));
            if (privilegeRequested is null || !privilegeRequested.Enabled)
            {   
                var unavailableFeatureContext = new PrivilegeAuthorizationDeniedContext();
                
                _tempDataProvider.SaveTempData(_httpContextAccessor.HttpContext, new Dictionary<string, object> {{"UnavailableFeatureContext", JsonConvert.SerializeObject(unavailableFeatureContext)}});
                return;
            }
            
            var contactPrivileges = await _contactsApiClient.GetContactPrivileges(Guid.Parse(userid));
            if (contactPrivileges.Any(cp => cp.Privilege.Key.Equals(requirement.Privilege, StringComparison.InvariantCultureIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                if (context.Resource is RouteEndpoint routeEndpoint)
                {
                    var controllerActionDescriptor = routeEndpoint.Metadata
                        .OfType<ControllerActionDescriptor>()
                        .SingleOrDefault();

                    if (controllerActionDescriptor != null)
                    {
                        var deniedContext = new PrivilegeAuthorizationDeniedContext
                        {
                            PrivilegeId = privilegeRequested.Id,
                            Controller = controllerActionDescriptor.ControllerName,
                            Action = controllerActionDescriptor.ActionName
                        };

                        _tempDataProvider.SaveTempData(_httpContextAccessor.HttpContext, new Dictionary<string, object> { { nameof(PrivilegeAuthorizationDeniedContext), JsonConvert.SerializeObject(deniedContext) } });
                        return;
                    }

                    context.Fail();
                }
            }
        }
    }
}