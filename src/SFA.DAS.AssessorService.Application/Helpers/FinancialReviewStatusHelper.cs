using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Helpers
{
    public static class FinancialReviewStatusHelper
    {
        public static bool IsFinancialExempt(bool? financialExempt, DateTime? financialDueDate, OrganisationType orgType)
        {
            bool exempt = financialExempt ?? false;

            bool orgTypeFinancialExempt = (orgType != null) && orgType.FinancialExempt;

            bool financialIsNotDue = (financialDueDate?.Date ?? DateTime.MinValue) > DateTime.Today;

            return exempt || financialIsNotDue || orgTypeFinancialExempt;
        }

        public static async Task SetFinancialReviewStatus(EpaOrganisation org, IRegisterQueryRepository registerQueryRepository)
        {
            if (null != org)
            {
                AssessorService.Api.Types.Models.AO.OrganisationType orgType = null;
                var orgTypes = await registerQueryRepository.GetOrganisationTypes();
                if (orgTypes != null)
                {
                    orgType = orgTypes.FirstOrDefault(x => x.Id == org.OrganisationTypeId);
                }

                org.FinancialReviewStatus = IsFinancialExempt(org.OrganisationData?.FHADetails?.FinancialExempt, org.OrganisationData?.FHADetails?.FinancialDueDate, orgType) ? ApplyTypes.FinancialReviewStatus.Exempt : ApplyTypes.FinancialReviewStatus.Required;
            }
        }
    }
}
