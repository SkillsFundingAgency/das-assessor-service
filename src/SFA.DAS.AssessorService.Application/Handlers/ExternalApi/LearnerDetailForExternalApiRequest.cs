using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi
{
    public class LearnerDetailForExternalApiRequest : IRequest<LearnerDetailForExternalApi>
    {
        public string Standard { get; }
        public long Uln { get; }

        public LearnerDetailForExternalApiRequest(string standard, long uln)
        {
            Standard = standard;
            Uln = uln;
        }
    }
}
