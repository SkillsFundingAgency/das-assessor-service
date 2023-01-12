using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentOrganisationsbyStandardRequest : IRequest<List<EpaOrganisation>>
    {
        public int StandardId { get; set; }
    }
}
