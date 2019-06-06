using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegePolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        private const string POLICY_PREFIX = "PrivilegePolicy_";
        
        public PrivilegePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX))
            {
                var privilege = policyName.Substring(POLICY_PREFIX.Length);
                
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PrivilegeRequirement(privilege));
                return Task.FromResult(policy.Build());
            }
            
            
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()=> 
            Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }

    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Privilege { get; }

        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }
    }
    
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
            var userid = _httpContextAccessor.HttpContext.User.FindFirst("UserId").Value;

            var contactPrivileges = await _contactsApiClient.GetContactPrivileges(Guid.Parse(userid));

            if (contactPrivileges.Any(cp => cp.Privilege.UserPrivilege.Equals(requirement.Privilege, StringComparison.InvariantCultureIgnoreCase)))
            {
                context.Succeed(requirement);    
            }
            else
            {
                var privilegeDenied = (await _contactsApiClient.GetPrivileges()).First(p => p.UserPrivilege.Equals(requirement.Privilege, StringComparison.InvariantCultureIgnoreCase));


                var controllerActionDescriptor = (context.Resource as AuthorizationFilterContext).ActionDescriptor as ControllerActionDescriptor;
                
                var deniedPrivilegeContext = new DeniedPrivilegeContext
                {
                    PrivilegeId = privilegeDenied.Id,
                    Controller = controllerActionDescriptor.ControllerName,
                    Action = controllerActionDescriptor.ActionName
                };
                
                _tempDataProvider.SaveTempData(_httpContextAccessor.HttpContext, new Dictionary<string, object> {{"DeniedPrivilegeContext", JsonConvert.SerializeObject(deniedPrivilegeContext)}});
            }
        }
    }

    public class DeniedPrivilegeContext
    {
        public Guid PrivilegeId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}