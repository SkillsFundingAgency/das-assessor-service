using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendEmailRequest : IRequest
    {
        public SendEmailRequest(string email, EMailTemplate emailTemplate, dynamic tokens)
        {
            EmailTemplate = emailTemplate;
            Email = email;
            Tokens = tokens;
        }

        public EMailTemplate EmailTemplate { get;  }
        public string Email { get; set; }
        public dynamic Tokens { get; }
    }
}
