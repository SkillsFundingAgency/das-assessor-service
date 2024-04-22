using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailTemplates
{
    public class GetEmailTemplateHandler : IRequestHandler<GetEmailTemplateRequest, EmailTemplateSummary>
    {
        private readonly IEMailTemplateQueryRepository _emailTemplateQueryRepository;
        private readonly IApiConfiguration _apiConfiguration;

        public GetEmailTemplateHandler(IEMailTemplateQueryRepository emailTemplateQueryRepository, IApiConfiguration apiConfiguration)
        {
            _emailTemplateQueryRepository = emailTemplateQueryRepository;
            _apiConfiguration = apiConfiguration;
        }

        public async Task<EmailTemplateSummary> Handle(GetEmailTemplateRequest request,
            CancellationToken cancellationToken)
        {
            //var emailTemplate = await _emailTemplateQueryRepository.GetEmailTemplate(request.TemplateName);
            var notificationEmailTemplate =  _apiConfiguration.NotificationTemplates.FirstOrDefault();
            var emailTemplate = new EmailTemplateSummary();
            emailTemplate.TemplateName = notificationEmailTemplate.TemplateName;
            emailTemplate.TemplateId = notificationEmailTemplate.TemplateId.ToString();
            emailTemplate.Recipients = notificationEmailTemplate.Recipients;
            return emailTemplate;
        }
    }
}