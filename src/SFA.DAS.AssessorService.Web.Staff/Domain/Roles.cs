using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.Staff.Domain
{
    public static class Roles
    {
        public const string RoleClaimType = "http://service/service";

        public const string CertificationTeam = "EPC";
        public const string OperationsTeam = "EPO";
        public const string AssessmentDeliveryTeam = "EPA"; // AAD
        public const string ProviderRiskAssuranceTeam = "EPR"; // FHA
        public const string RegisterViewOnlyTeam = "EPV";
        public const string RoatpGatewayTeam = "APR";
        
        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(CertificationTeam)
                   || user.IsInRole(OperationsTeam)
                   || user.IsInRole(AssessmentDeliveryTeam)
                   || user.IsInRole(ProviderRiskAssuranceTeam)
                   || user.IsInRole(RegisterViewOnlyTeam)
                   || user.IsInRole(RoatpGatewayTeam);
        }

        public static bool HasRoatpRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoatpGatewayTeam);
        }
    }
}
