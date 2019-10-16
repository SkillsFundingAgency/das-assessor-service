using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderNonApprovedStandardDetailsRequest : IRequest<GetOppFinderNonApprovedStandardDetailsResponse>
    {
        public string StandardReference { get; set; }
    }
}
