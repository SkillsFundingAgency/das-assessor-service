using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public class ValidationRequest : IRequest<bool>
    {


        public string ValidationType { get; set; }
        public string ValidationString { get; set; }
        public string ValidationMatchValue { get; set; }

    }
}
