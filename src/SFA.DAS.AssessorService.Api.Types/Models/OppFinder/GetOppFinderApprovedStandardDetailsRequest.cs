using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderApprovedStandardDetailsRequest : IRequest<GetOppFinderApprovedStandardDetailsResponse>
    {
        public string StandardReference { get; set; }
    }
}
