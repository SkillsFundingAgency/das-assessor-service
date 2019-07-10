using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

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
}