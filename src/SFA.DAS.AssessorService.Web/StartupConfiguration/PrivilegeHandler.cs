using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeHandler : AuthorizationHandler<PrivilegeRequirement>
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataProvider _tempDataProvider;

        public PrivilegeHandler(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, ITempDataProvider tempDataProvider)
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

            var controllerActionDescriptor = (context.Resource as AuthorizationFilterContext).ActionDescriptor as ControllerActionDescriptor;
            
            var privilegeRequested = (await _contactsApiClient.GetPrivileges()).FirstOrDefault(p => p.Key.Equals(requirement.Privilege, StringComparison.InvariantCultureIgnoreCase));
            if (privilegeRequested is null || !privilegeRequested.Enabled)
            {   
                var unavailableFeatureContext = new DeniedPrivilegeContext();
                
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
                var deniedPrivilegeContext = new DeniedPrivilegeContext
                {
                    PrivilegeId = privilegeRequested.Id,
                    Controller = controllerActionDescriptor.ControllerName,
                    Action = controllerActionDescriptor.ActionName
                };
                
                _tempDataProvider.SaveTempData(_httpContextAccessor.HttpContext, new Dictionary<string, object> {{"DeniedPrivilegeContext", JsonConvert.SerializeObject(deniedPrivilegeContext)}});
            }
        }
    }
}