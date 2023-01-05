using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Models
{
    public class ApprovedStandardsWithVersionsViewModel
    {        
        public PaginatedList<GetEpaoRegisteredStandardsResponse> ApprovedStandardsWithVersions { get; set; }

        public bool FinancialInfoStage1Expired { get; set; }
        public string FinancialAssessmentUrl { get; set; }

        public ApprovedStandardsWithVersionsViewModel()
        {
            ApprovedStandardsWithVersions =
                new PaginatedList<GetEpaoRegisteredStandardsResponse>(new List<GetEpaoRegisteredStandardsResponse>(), 0,
                    1, 1);
        }
    }
}
