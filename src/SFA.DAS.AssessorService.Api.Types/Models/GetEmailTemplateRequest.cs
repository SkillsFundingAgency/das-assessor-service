using MediatR;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Api.Types.Models
{ 
    public class GetEmailTemplateRequest : IRequest<EmailTemplateSummary>
    {
        public string TemplateName { get; set; }
    }
}
