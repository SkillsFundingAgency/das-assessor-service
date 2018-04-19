using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class GetEMailTemplateRequest : IRequest<EMailTemplate>
    {
        public string TemplateName { get; set; }
    }
}
