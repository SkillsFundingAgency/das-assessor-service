using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentsRequest : IRequest<GetAssessmentsResponse>
    {
        public string StandardReference {get; set;}
        public long Ukprn { get; set; }
    }
}
