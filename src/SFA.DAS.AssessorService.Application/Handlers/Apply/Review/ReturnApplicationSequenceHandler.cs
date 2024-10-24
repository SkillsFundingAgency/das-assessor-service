using MediatR;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class ReturnApplicationSequenceHandler : IRequestHandler<ReturnApplicationSequenceRequest, Unit>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IMediator _mediator;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IApiConfiguration _config;

        private const string SERVICE_NAME = "Apprenticeship assessment service";
        private const string SERVICE_TEAM = "Apprenticeship assessment service team";

        public ReturnApplicationSequenceHandler(IApplyRepository applyRepository, IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository, IApiConfiguration config, IMediator mediator)
        {
            _applyRepository = applyRepository;
            _mediator = mediator;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _config = config;
        }

        public async Task<Unit> Handle(ReturnApplicationSequenceRequest request, CancellationToken cancellationToken)
        {
            switch (request.ReturnType)
            {
                case "ReturnWithFeedback":
                    await ReturnSequenceWithFeedback(request);
                    break;
                case "Approve":
                case "ApproveWithFeedback":
                    await ApproveSequence(request);
                    break;
                default:
                    await DeclineSequence(request);
                    break;
            }

            await NotifyContact(request.ApplicationId, request.SequenceId, request.ReturnType, cancellationToken);

            return Unit.Value;
        }

        private async Task ReturnSequenceWithFeedback(ReturnApplicationSequenceRequest request)
        {
            await _applyRepository.ReturnApplicationSequence(request.ApplicationId, request.SequenceId, ApplicationSequenceStatus.FeedbackAdded, request.ReturnedBy);
        }

        private async Task ApproveSequence(ReturnApplicationSequenceRequest request)
        {
            await _applyRepository.ReturnApplicationSequence(request.ApplicationId, request.SequenceId, ApplicationSequenceStatus.Approved, request.ReturnedBy);
        }

        private async Task DeclineSequence(ReturnApplicationSequenceRequest request)
        {
            await _applyRepository.ReturnApplicationSequence(request.ApplicationId, request.SequenceId, ApplicationSequenceStatus.Declined, request.ReturnedBy);
        }

        private async Task NotifyContact(Guid applicationId, int sequenceNo, string returnType, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApply(applicationId);

            if (application != null)
            {
                var loginLink = $"{_config.ServiceLink}/Account/SignIn";

                if (sequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO)
                {
                    var lastSubmission = application.ApplyData?.Apply.LatestInitSubmission;
                    if (lastSubmission != null)
                    {
                        var contactToNotify = await _contactQueryRepository.GetContactById(lastSubmission.SubmittedBy);

                        var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.APPLY_EPAO_UPDATE);
                        await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                            new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.DisplayName, LoginLink = loginLink }), cancellationToken);
                    }
                }
                else if (sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO)
                {
                    var lastSubmission = application.ApplyData?.Apply.LatestStandardSubmission;
                    if (lastSubmission != null)
                    {
                        var standardName = application.ApplyData?.Apply.StandardName ?? string.Empty;
                        var contactToNotify = await _contactQueryRepository.GetContactById(lastSubmission.SubmittedBy);

                        var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.APPLY_EPAO_RESPONSE);
                        await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                            new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.DisplayName, standard = standardName, LoginLink = loginLink }), cancellationToken);
                    }
                }
                else if(returnType == "ReturnWithFeedback" && 
                        (sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO || 
                         sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO))
                {
                    var lastSubmission = sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO
                        ? application.ApplyData?.Apply.LatestOrganisationWithdrawalSubmission
                        : application.ApplyData?.Apply.LatestStandardWithdrawalSubmission;

                    if (lastSubmission != null)
                    {
                        var contactToNotify = await _contactQueryRepository.GetContactById(lastSubmission.SubmittedBy);
                        var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOWithdrawalFeedbackNotification);

                        await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                            new
                            {
                                ServiceName = SERVICE_NAME, 
                                ServiceTeam = SERVICE_TEAM,
                                Contact = contactToNotify.DisplayName, 
                                LoginLink = loginLink
                            }), cancellationToken);
                    }
                }
                else if (sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO && returnType == "Approve")
                {
                    var lastSubmission = application.ApplyData?.Apply.LatestOrganisationWithdrawalSubmission;
                    if (lastSubmission != null)
                    {                        
                        var contactToNotify = await _contactQueryRepository.GetContactById(lastSubmission.SubmittedBy);

                        var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAORegisterWithdrawalApproval);
                        await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                            new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.DisplayName,  LoginLink = loginLink }), cancellationToken);
                    }
                }
                else if (sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO && returnType == "Approve")
                {
                    var lastSubmission = application.ApplyData?.Apply.LatestStandardWithdrawalSubmission;
                    if (lastSubmission != null)
                    {
                        var standardName = application.ApplyData?.Apply.StandardName ?? string.Empty;
                        var standardReferance = application.ApplyData?.Apply.StandardReference ?? string.Empty;
                        var contactToNotify = await _contactQueryRepository.GetContactById(lastSubmission.SubmittedBy);

                        var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOStandardWithdrawalApproval);
                        await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                            new { ServiceName = SERVICE_NAME, ServiceTeam = SERVICE_TEAM, Contact = contactToNotify.DisplayName, StandardName = standardName, StandardReference = standardReferance,  LoginLink = loginLink }), cancellationToken);
                    }
                }
            }
        }
    }
}
