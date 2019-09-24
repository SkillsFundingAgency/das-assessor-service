using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationRequest, Guid>
    {
        private readonly IApplyRepository _applyRepository;

        public CreateApplicationHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Guid> Handle(CreateApplicationRequest request, CancellationToken cancellationToken)
        {
           
            var applyData = new ApplyData
            {
                Sequences = request.listOfApplySequences
            };
            await AddApplyDataWithSubmissionInfo(applyData, request);
            var application = new Domain.Entities.Application
            {
                ApplyData = applyData,
                ApplicationId = request.ApplicationId,
                StandardCode = request.StandardCode,
                ApplicationStatus = request.ApplicationStatus,
                ReviewStatus = ApplicationReviewStatus.Draft,
                OrganisationId = request.OrganisationId,
                CreatedBy = request.UserId.ToString()
            };

            var response = await _applyRepository.CreateApplication(application, ApplicationStatus.InProgress);
            return response;
        }

        private async Task AddApplyDataWithSubmissionInfo(ApplyData applyData, CreateApplicationRequest request)
        {
            applyData.Apply = new ApplyTypes.Apply();
            applyData.Apply.ReferenceNumber = await CreateReferenceNumber(request.ReferenceFormat);
            foreach (var sequence in applyData.Sequences)
            {

                if (sequence.SequenceNo == 1)
                {
                    applyData.Apply.InitSubmissions = new List<InitSubmission>();
                    applyData.Apply.InitSubmissionCount = 0;
                    applyData.Apply.LatestInitSubmissionDate = null;
                }
                else if (sequence.SequenceNo == 2)
                {
                    applyData.Apply.StandardCode = request.StandardCode;
                    applyData.Apply.StandardName = request.StandardName;
                    applyData.Apply.StandardReference = request.StandardReference;

                   
                    applyData.Apply.StandardSubmissionsCount = 0;
                    applyData.Apply.LatestStandardSubmissionDate = null;
                    applyData.Apply.StandardSubmissionClosedDate = null;
                    applyData.Apply.StandardSubmissionFeedbackAddedDate = null;
                }
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
    }
}