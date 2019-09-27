using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OppFinderExpressionOfInterestRequest : IRequest<bool>
    {
        public string StandardReference { get; set; }
        public string Email { get; set; }
        public string OrganisationName { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
    }
}
