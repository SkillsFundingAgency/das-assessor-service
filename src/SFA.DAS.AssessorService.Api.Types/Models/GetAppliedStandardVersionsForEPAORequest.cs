using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAppliedStandardVersionsForEpaoRequest : IRequest<IEnumerable<AppliedStandardVersion>>
    {
        public string OrganisationId { get; set; }

        public string StandardReference { get; set; }
    }
}
