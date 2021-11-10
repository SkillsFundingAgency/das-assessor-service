using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsFiltersRequest : IRequest<EpaoPipelineStandardsFiltersResponse>
    {
        public string EpaoId { get; private set; }

        public EpaoPipelineStandardsFiltersRequest(string epaoId)
        {
            EpaoId = epaoId;
        }
    }
}
