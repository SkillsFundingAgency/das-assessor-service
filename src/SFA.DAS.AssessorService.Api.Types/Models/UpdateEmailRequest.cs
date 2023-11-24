using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateEmailRequest : IRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NewEmail { get; set; }
    }
}
