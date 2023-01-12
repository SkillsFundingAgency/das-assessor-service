using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchAssessmentOrganisationsRequest : IRequest<List<AssessmentOrganisationSummary>>
    {
        public string SearchTerm { get; set; }
    }
}