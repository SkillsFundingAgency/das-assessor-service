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
                if(application.ApplyData == null)
                {
                    application.ApplyData = new ApplyData();
                }

                if(application.ApplyData.Apply == null)
                {
                    application.ApplyData.Apply = new ApplyTypes.Apply();
                }

                if (string.IsNullOrWhiteSpace(application.ApplyData.Apply.ReferenceNumber))
                {
                    application.ApplyData.Apply.ReferenceNumber = await CreateReferenceNumber(request.ReferenceFormat);
                }

                AddSequenceToApplyData(application.ApplyData, request.Sequence); // NOTE: Find out why we are doing this
                AddSubmissionInfoToApplyData(application.ApplyData, request);

                application.StandardCode = request.StandardCode;
                application.ReviewStatus = ApplicationReviewStatus.New;
                application.UpdatedBy = request.UserId.ToString();
                application.UpdatedAt = DateTime.UtcNow;

                UpdateApplicationStatus(application, request);
                await _applyRepository.SubmitApplicationSequence(application);

                application = await _applyRepository.GetApplication(request.ApplicationId);
                var reference = application.ApplyData.Apply.ReferenceNumber;
                var standard = application.ApplyData.Apply.StandardName;
                await NotifyContact(request.Email, request.ContactName, request.Sequence.SequenceNo, reference, standard, cancellationToken);

                return true;
            }

            return false;
        }

        private void AddSequenceToApplyData(ApplyData applyData, ApplySequence sequence)
        {
            if (applyData.Sequences == null)
                applyData.Sequences = new List<ApplySequence>();

            if (applyData.Sequences.Any(x => x.SequenceNo == sequence.SequenceNo))
            {
                foreach (var applySequence in applyData.Sequences.Where(x => x.SequenceNo == sequence.SequenceNo))
                {
                    applySequence.Status = applySequence.Status == ApplicationSectionStatus.Submitted ? ApplicationSequenceStatus.Resubmitted : sequence.Status;
                    applySequence.Sections = sequence.Sections;
                    applySequence.SequenceNo = sequence.SequenceNo;
                    applySequence.NotRequired = sequence.NotRequired;
                }
            }
            else
            {
                applyData.Sequences.Add(sequence);
            }
        }

        private void AddSubmissionInfoToApplyData(ApplyData applyData, SubmitApplicationRequest request)
        {
            if (applyData.Apply == null)
                applyData.Apply = new ApplyTypes.Apply();

            if (request.Sequence.SequenceNo == 1)
            {
                if (applyData.Apply.InitSubmissions == null)
                {
                    applyData.Apply.InitSubmissions = new List<Submission>();
                }

                var initSubmission = new Submission
                {
                    SubmittedAt = DateTime.UtcNow,
                    SubmittedBy = request.UserId,
                    SubmittedByEmail = request.Email
                };

                applyData.Apply.InitSubmissions.Add(initSubmission);
                applyData.Apply.InitSubmissionCount = applyData.Apply.InitSubmissions.Count;
                applyData.Apply.LatestInitSubmissionDate = initSubmission.SubmittedAt;
                // TODO: why these???
                //applyData.Apply.InitSubmissionClosedDate = request.StandardSubmissionClosedDate;
                //applyData.Apply.InitSubmissionFeedbackAddedDate = request.StandardSubmissionFeedbackAddedDate;
            }
            else if (request.Sequence.SequenceNo == 2)
            {
                applyData.Apply.StandardCode = request.StandardCode;
                applyData.Apply.StandardName = request.StandardName;
                applyData.Apply.StandardReference = request.StandardReference;

                if (applyData.Apply.StandardSubmissions == null)
                {
                    applyData.Apply.StandardSubmissions = new List<Submission>();
                }

                var standardSubmission = new Submission
                {
                    SubmittedAt = DateTime.UtcNow,
                    SubmittedBy = request.UserId,
                    SubmittedByEmail = request.Email
                };

                applyData.Apply.StandardSubmissions.Add(standardSubmission);
                applyData.Apply.StandardSubmissionsCount = applyData.Apply.StandardSubmissions.Count;
                applyData.Apply.LatestStandardSubmissionDate = standardSubmission.SubmittedAt;
                // TODO: why these???
                //applyData.Apply.StandardSubmissionClosedDate = request.StandardSubmissionClosedDate;
                //applyData.Apply.StandardSubmissionFeedbackAddedDate = request.StandardSubmissionFeedbackAddedDate;
            }

        }

        private void UpdateApplicationStatus(Domain.Entities.Apply application, SubmitApplicationRequest request)
        {
            // Always default it to submitted
            application.ApplicationStatus = ApplicationStatus.Submitted;

            var applyData = application.ApplyData;

            if (request.Sequence.SequenceNo == 1)
            {
                application.ApplicationStatus = (applyData.Apply.InitSubmissions.Count == 1) ? ApplicationStatus.Submitted : ApplicationStatus.Resubmitted;

                var closedFinanicalStatuses = new List<string> { FinancialReviewStatus.Approved, FinancialReviewStatus.Exempt };

                if (!closedFinanicalStatuses.Contains(application.FinancialReviewStatus))
                {
                    application.FinancialReviewStatus = FinancialReviewStatus.New;
                }
            }
            else if (request.Sequence.SequenceNo == 2)
            {
                application.ApplicationStatus = (applyData.Apply.StandardSubmissions.Count == 1) ? ApplicationStatus.Submitted : ApplicationStatus.Resubmitted;
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
