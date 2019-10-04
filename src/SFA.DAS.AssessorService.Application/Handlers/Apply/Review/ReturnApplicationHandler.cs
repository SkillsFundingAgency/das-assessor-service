using MediatR;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class ReturnApplicationHandler : IRequestHandler<ReturnApplicationRequest>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IMediator _mediator;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IWebConfiguration _config;

        private const string SERVICE_NAME = "Apprenticeship assessment service";
        private const string SERVICE_TEAM = "Apprenticeship assessment service team";

        public ReturnApplicationHandler(IApplyRepository applyRepository, IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactRepository contactRepository, IWebConfiguration config, IMediator mediator)
        {
            _applyRepository = applyRepository;
            _mediator = mediator;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactRepository = contactRepository;
            _config = config;
        }

        public async Task<Unit> Handle(ReturnApplicationRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Migrate code over");

            await NotifyContact(request.ApplicationId, request.SequenceId, cancellationToken);

            return Unit.Value;
        }

        private async Task NotifyContact(Guid applicationId, int sequenceNo, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApplication(applicationId);
            var standard = application.ApplyData?.Apply.StandardName ?? string.Empty;
            var loginLink = $"{_config.ServiceLink}/Account/SignIn";

            if (sequenceNo == 1)
            {
                var lastInitSubmission = application.ApplyData?.Apply.InitSubmissions.OrderByDescending(sub => sub.SubmittedAt).FirstOrDefault();

                if (lastInitSubmission != null)
                {                 
                    var contactToNotify = await _contactRepository.GetContact(lastInitSubmission.SubmittedByEmail);

                    var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.APPLY_EPAO_UPDATE);
                    await _mediator.Send(new SendEmailRequest(contactToNotify.Email , emailTemplate, 
                        new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.GivenNames, LoginLink = loginLink }), cancellationToken);
                }
            }
            else if (sequenceNo == 2)
            {
                var lastStandardSubmission = application.ApplyData?.Apply.StandardSubmissions.OrderByDescending(sub => sub.SubmittedAt).FirstOrDefault();

                if (lastStandardSubmission != null)
                {
                    var contactToNotify = await _contactRepository.GetContact(lastStandardSubmission.SubmittedByEmail);
                    
                    var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.APPLY_EPAO_RESPONSE);
                    await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                        new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.GivenNames, standard, LoginLink = loginLink }), cancellationToken);
                }
            }
        }
    }
}
