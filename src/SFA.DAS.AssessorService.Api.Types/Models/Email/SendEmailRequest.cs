using MediatR;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendEmailRequest : IRequest
    {
        public SendEmailRequest(string email, EmailTemplateSummary emailTemplateSummary, dynamic tokens)
        {
            EmailTemplateSummary = emailTemplateSummary;
            Email = email;
            Tokens = tokens;
        }

        public EmailTemplateSummary EmailTemplateSummary { get;  }
        public string Email { get; set; }
        public dynamic Tokens { get; }
    }
}
