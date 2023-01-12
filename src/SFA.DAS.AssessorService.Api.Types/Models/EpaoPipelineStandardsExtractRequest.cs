using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsExtractRequest : IRequest<List<EpaoPipelineStandardsExtractResponse>>
    {
        public EpaoPipelineStandardsExtractRequest(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId)
        {
            EpaoId = epaoId;
            StandardFilterId = standardFilterId;
            ProviderFilterId = providerFilterId;
            EPADateFilterId = epaDateFilterId;
        }

        public string EpaoId { get; private set; }
        public string StandardFilterId { get; private set; }
        public string ProviderFilterId { get; private set; }
        public string EPADateFilterId { get; private set; }
    }
}
