using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsExtractRequest: IRequest<List<EpaoPipelineStandardsExtractResponse>>
    {
        public EpaoPipelineStandardsExtractRequest(string epaoId)
        {
            EpaoId = epaoId;
        }

        public string EpaoId { get; private set; }
    }
}
