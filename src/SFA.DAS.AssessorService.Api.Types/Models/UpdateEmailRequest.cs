using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateEmailRequest : IRequest
    {
        public string GovUkIdentifier { get; set; }
        public string NewEmail { get; set; }
    }
}
