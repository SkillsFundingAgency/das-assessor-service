using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.Staff.Domain
{
    public static class Roles
    {
        public const string RoleClaimType = "http://service/service";

        public const string CertificationTeam = "EPC";
        public const string OperationsTeam = "EPO";
        public const string AssessmentDeliveryTeam = "EPA";
        public const string ProviderRiskAssuranceTeam = "EPR";
        public const string RegisterViewOnlyTeam = "RVO";

        public static bool HasValidRole(this ClaimsPrincipal User)
        {
            return User.IsInRole(CertificationTeam)
                   || User.IsInRole(OperationsTeam)
                   || User.IsInRole(AssessmentDeliveryTeam)
                   || User.IsInRole(ProviderRiskAssuranceTeam)
                   || User.IsInRole(RegisterViewOnlyTeam);

        }
    }
}
