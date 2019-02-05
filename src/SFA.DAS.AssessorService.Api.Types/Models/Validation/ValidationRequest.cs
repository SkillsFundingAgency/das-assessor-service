using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public class ValidationRequest : IRequest<bool>
    {


        public string Type { get; set; }
        public string Value { get; set; }
        public string MatchCriteria { get; set; }

    }
}
