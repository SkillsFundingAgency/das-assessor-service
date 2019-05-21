using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailTemplates
{
    public class GetEmailTemplateHandler : IRequestHandler<GetEmailTemplateRequest, EMailTemplate>
    {
        private readonly IEMailTemplateQueryRepository _emailTemplateQueryRepository;

        public GetEmailTemplateHandler(IEMailTemplateQueryRepository emailTemplateQueryRepository)
        {
            _emailTemplateQueryRepository = emailTemplateQueryRepository;
        }

        public async Task<EMailTemplate> Handle(GetEmailTemplateRequest request,
            CancellationToken cancellationToken)
        {
            var emailTemplate = await _emailTemplateQueryRepository.GetEmailTemplate(request.TemplateName);
            return emailTemplate;
        }
    }
}