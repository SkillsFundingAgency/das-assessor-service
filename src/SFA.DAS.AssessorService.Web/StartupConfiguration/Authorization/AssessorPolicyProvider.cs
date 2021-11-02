using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class AssessorPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        
        public AssessorPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PrivilegeAuthorizeAttribute.POLICY_PREFIX))
            {
                var privilege = policyName.Substring(PrivilegeAuthorizeAttribute.POLICY_PREFIX.Length);
                
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PrivilegeRequirement(privilege));
                return Task.FromResult(policy.Build());
            }
            else if(policyName.StartsWith(ApplicationAuthorizeAttribute.POLICY_PREFIX))
            {
                var routeId = policyName.Substring(ApplicationAuthorizeAttribute.POLICY_PREFIX.Length);

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new ApplicationRequirement(routeId));
                return Task.FromResult(policy.Build());
            }
            
            
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()=> 
            Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            // @ToDo: GetFallbackPolicyAsync is a new interface method as a result of the .Net Core 3.1 migration.
            // gets triggered due to having to add .UseAuthorization between UseRouting and UseEndpoints
            // copy GetDefaultPolicyAsync and see if that works
            
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}