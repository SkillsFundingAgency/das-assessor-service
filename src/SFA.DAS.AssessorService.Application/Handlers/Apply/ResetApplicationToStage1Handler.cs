using MediatR;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class ResetApplicationToStage1Handler : IRequestHandler<ResetApplicationToStage1Request, bool>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IMediator _mediator;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;

        private const string SERVICE_NAME = "Apprenticeship assessment service";
        private const string SERVICE_TEAM = "Apprenticeship assessment service team";

        public ResetApplicationToStage1Handler(IApplyRepository applyRepository, IMediator mediator, 
            IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository)
        {
            _applyRepository = applyRepository;
            _mediator = mediator;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<bool> Handle(ResetApplicationToStage1Request request, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApply(request.Id);
            if (application != null)
            {
                var lastSubmission = application.ApplyData?.Apply?.LatestStandardSubmission
                    ?? application.ApplyData?.Apply?.LatestInitSubmission;

                if (lastSubmission != null)
                {
                    var result = await _applyRepository.ResetApplicatonToStage1(request.Id, lastSubmission.SubmittedBy);
                    if (result)
                    {
                        await NotifyContact(request.Id, lastSubmission.SubmittedBy, application.ApplyData?.Apply?.StandardWithReference, cancellationToken);
                        return result;
                    }
                }
            }

            return false;
        }

        private async Task NotifyContact(Guid id, Guid submittedBy, string standardWithReference, CancellationToken cancellationToken)
        {
            var contactToNotify = await _contactQueryRepository.GetContactById(submittedBy);
            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOCancelApplication);
            
            await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.DisplayName, StandardWithReference = standardWithReference }), cancellationToken);
        }
    }
}
