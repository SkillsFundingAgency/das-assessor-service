using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.EMailTemplates
{
    public class GetEMailTemplatesHandler : IRequestHandler<GetEMailTemplateRequest, EMailTemplate>
    {
        private readonly IEMailTemplateQueryRepository _emailTemplateQueryRepository;

        public GetEMailTemplatesHandler(IEMailTemplateQueryRepository emailTemplateQueryRepository)
        {
            _emailTemplateQueryRepository = emailTemplateQueryRepository;
        }

        public async Task<EMailTemplate> Handle(GetEMailTemplateRequest request,
            CancellationToken cancellationToken)
        {
            var emailTemplate = await _emailTemplateQueryRepository.GetEMailTemplate(request.TemplateName);
            return emailTemplate;
        }
    }
}