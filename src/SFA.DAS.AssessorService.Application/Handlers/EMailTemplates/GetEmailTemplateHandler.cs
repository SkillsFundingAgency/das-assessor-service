﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Handlers.EmailTemplates
{
    public class GetEmailTemplateHandler : IRequestHandler<GetEmailTemplateRequest, EmailTemplateSummary>
    {
        private readonly IEMailTemplateQueryRepository _emailTemplateQueryRepository;

        public GetEmailTemplateHandler(IEMailTemplateQueryRepository emailTemplateQueryRepository)
        {
            _emailTemplateQueryRepository = emailTemplateQueryRepository;
        }

        public async Task<EmailTemplateSummary> Handle(GetEmailTemplateRequest request,
            CancellationToken cancellationToken)
        {
            var emailTemplate = await _emailTemplateQueryRepository.GetEmailTemplate(request.TemplateName);
            return emailTemplate;
        }
    }
}