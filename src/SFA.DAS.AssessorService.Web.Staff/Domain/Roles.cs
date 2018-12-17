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
        public const string RegisterViewOnlyTeam = "EPV";
        public const string FinancialHealthAssessmentTeam = "FHA";
        public const string ApprenticeshipAssuranceDeliveryTeam = "AAD";

        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(CertificationTeam)
                   || user.IsInRole(OperationsTeam)
                   || user.IsInRole(AssessmentDeliveryTeam)
                   || user.IsInRole(ProviderRiskAssuranceTeam)
                   || user.IsInRole(RegisterViewOnlyTeam)
                   || user.IsInRole(FinancialHealthAssessmentTeam)
                   || user.IsInRole(ApprenticeshipAssuranceDeliveryTeam);

        }
    }
}
