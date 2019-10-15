using MediatR;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class SubmitApplicationHandler : IRequestHandler<SubmitApplicationRequest, bool>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IMediator _mediator;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;

        public SubmitApplicationHandler(IApplyRepository applyRepository, IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator)
        {
            _applyRepository = applyRepository;
            _mediator = mediator;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
        }

        public async Task<bool> Handle(SubmitApplicationRequest request, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApplication(request.ApplicationId);

            if(application != null)
            {
                var applyData = application.ApplyData;
                if (applyData == null)
                    applyData = new ApplyData();

                AddApplyDataWithSequence(applyData, request.Sequence);
                await AddApplyDataWithSubmissionInfo(applyData, request);

                application.ApplyData = applyData;
                application.StandardCode = request.StandardCode;
                application.ReviewStatus = ApplicationReviewStatus.New;
                application.UpdatedBy = request.UserId.ToString();
                UpdateApplicationStatus(applyData, request, application);
                await _applyRepository.SubmitApplicationSequence(application);

                application = await _applyRepository.GetApplication(request.ApplicationId);
                var reference = application.ApplyData.Apply.ReferenceNumber;
                var standard = application.ApplyData.Apply.StandardName;
                await NotifyContact(request.Email, request.ContactName, request.Sequence.SequenceNo, reference, standard, cancellationToken);

                return true;
            }

            return false;
        }

        private void AddApplyDataWithSequence(ApplyData applyData, ApplySequence sequence)
        {
            if (applyData.Sequences == null)
                applyData.Sequences = new List<ApplySequence>();
            if (applyData.Sequences.Any(x => x.SequenceNo == sequence.SequenceNo))
            {
                applyData.Sequences.Where(x => x.SequenceNo == sequence.SequenceNo)
                   .Select(applySequence =>
                   {
                       applySequence.Status = applySequence.Status == ApplicationSectionStatus.Submitted? ApplicationSequenceStatus.Resubmitted:sequence.Status;
                       applySequence.Sections = sequence.Sections;
                       applySequence.SequenceNo = sequence.SequenceNo;
                       applySequence.NotRequired = sequence.NotRequired;
                       return applySequence;
                   }).ToList();
            }
            else
            {
                applyData.Sequences.Add(sequence);
            }
        }

        private async Task AddApplyDataWithSubmissionInfo(ApplyData applyData, SubmitApplicationRequest request)
        {
            if (applyData.Apply == null)
                applyData.Apply = new ApplyTypes.Apply();

            if (string.IsNullOrWhiteSpace(applyData.Apply.ReferenceNumber))
            {
                applyData.Apply.ReferenceNumber = await CreateReferenceNumber(request.ReferenceFormat);
            }

            if (request.Sequence.SequenceNo == 1)
            {
                if (applyData.Apply.InitSubmissions == null)
                {
                    applyData.Apply.InitSubmissions = new List<InitSubmission>();
                }

                var submission = new InitSubmission
                {
                    SubmittedAt = DateTime.UtcNow,
                    SubmittedBy = request.UserId,
                    SubmittedByEmail = request.Email
                };

                applyData.Apply.InitSubmissions.Add(submission);
                applyData.Apply.InitSubmissionCount = applyData.Apply.InitSubmissions.Count;
                applyData.Apply.LatestInitSubmissionDate = submission.SubmittedAt;
            }
            else if (request.Sequence.SequenceNo == 2)
            {
                applyData.Apply.StandardCode = request.StandardCode;
                applyData.Apply.StandardName = request.StandardName;
                applyData.Apply.StandardReference = request.StandardReference;

                if (applyData.Apply.StandardSubmissions == null)
                {
                    applyData.Apply.StandardSubmissions = new List<StandardSubmission>();
                }

                var submission = new StandardSubmission
                {
                    SubmittedAt = DateTime.UtcNow,
                    SubmittedBy = request.UserId,
                    SubmittedByEmail = request.Email
                };

                applyData.Apply.StandardSubmissions.Add(submission);
                applyData.Apply.StandardSubmissionsCount = applyData.Apply.StandardSubmissions.Count;
                applyData.Apply.LatestStandardSubmissionDate = submission.SubmittedAt;
                applyData.Apply.StandardSubmissionClosedDate = request.StandardSubmissionClosedDate;
                applyData.Apply.StandardSubmissionFeedbackAddedDate = request.StandardSubmissionFeedbackAddedDate;
            }

        }

        private void UpdateApplicationStatus(ApplyData applyData, SubmitApplicationRequest request, Domain.Entities.Apply apply)
        {
            // Always default it to submitted
            apply.ApplicationStatus = ApplicationStatus.Submitted;

            if (request.Sequence.SequenceNo == 1)
            {
                apply.ApplicationStatus = (applyData.Apply.InitSubmissions.Count == 1) ? ApplicationStatus.Submitted : ApplicationStatus.Resubmitted;

                var closedFinanicalStatuses = new List<string> { FinancialReviewStatus.Approved, FinancialReviewStatus.Exempt };

                if (!closedFinanicalStatuses.Contains(apply.FinancialReviewStatus))
                {
                    apply.FinancialReviewStatus = FinancialReviewStatus.New;
                }
            }
            else if (request.Sequence.SequenceNo == 2)
            {
                apply.ApplicationStatus = (applyData.Apply.StandardSubmissions.Count == 1) ? ApplicationStatus.Submitted : ApplicationStatus.Resubmitted;
            }
        }


        private async Task<string> CreateReferenceNumber(string referenceFormat)
        {
            var referenceNumber = string.Empty;

            var seq = await _applyRepository.GetNextAppReferenceSequence();

            if (seq > 0 && !string.IsNullOrEmpty(referenceFormat))
            {
                referenceNumber = string.Format($"{referenceFormat}{seq:D6}");
            }

            return referenceNumber;
        }

        private async Task NotifyContact(string email, string contactname, int sequenceNo, string reference, string standard, CancellationToken cancellationToken)
        {
            if (sequenceNo == 1)
            {
                var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.ApplyEPAOInitialSubmission);
                await _mediator.Send(new SendEmailRequest(email, emailTemplate, new { contactname, reference }), cancellationToken);

            }
            else if (sequenceNo == 2)
            {
                var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.ApplyEPAOStandardSubmission);
                await _mediator.Send(new SendEmailRequest(email, emailTemplate, new { reference, standard }), cancellationToken);
            }
        }

    }
}
