using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
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
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationRequest, Guid>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public CreateApplicationHandler(IOrganisationQueryRepository organisationQueryRepository, 
            IRegisterQueryRepository registerQueryRepository, IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _registerQueryRepository = registerQueryRepository;
        }

        public async Task<Guid> Handle(CreateApplicationRequest request, CancellationToken cancellationToken)
        {
            var org = await _organisationQueryRepository.Get(request.OrganisationId);
            var orgTypes = await _registerQueryRepository.GetOrganisationTypes();
            var orgType = orgTypes.FirstOrDefault(x => x.Id == org.OrganisationTypeId);
            DisableSequencesAndSectionsAsAppropriate(org, request.listOfApplySequences, orgType);

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
                FinancialReviewStatus = IsFinancialExempt(org.OrganisationData?.FHADetails, orgType) ? FinancialReviewStatus.Exempt: FinancialReviewStatus.Draft,
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

        private bool DisableSequencesAndSectionsAsAppropriate(Domain.Entities.Organisation org, 
            List<ApplySequence> sequences, OrganisationType orgType)
        {
            bool isEpao = IsOrganisationOnEPAORegister(org);
            if (isEpao)
            {
                RemoveSectionsOneAndTwo(sequences.Single(x => x.SequenceNo == 1));
            }

            bool isFinancialExempt = IsFinancialExempt(org.OrganisationData?.FHADetails,orgType);
            if (isFinancialExempt)
            {
                RemoveSectionThree(sequences.Single(x => x.SequenceNo == 1));
            }

            if (isEpao && isFinancialExempt)
            {
                RemoveSequenceOne(sequences);
            }
            return isFinancialExempt;
        }

        private static bool IsOrganisationOnEPAORegister(Domain.Entities.Organisation org)
        {
            if (org is null) return false;

            return org.OrganisationData.RoEPAOApproved;
        }

        private void RemoveSequenceOne(List<ApplySequence> sequences)
        {
            var stage1 = sequences.Single(seq => seq.SequenceNo == 1);
            stage1.IsActive = false;
            stage1.NotRequired = true;
            stage1.Status = ApplicationSequenceStatus.Approved;

            sequences.Single(seq => seq.SequenceNo == 2).IsActive = true;
        }

        private void RemoveSectionThree(ApplySequence applySequence)
        {
            foreach (var applySection in applySequence.Sections.Where( x=> x.SectionNo == 3))
            {
                applySection.NotRequired = true;
                applySection.Status = ApplicationSectionStatus.Evaluated;
            }
        }

        private void RemoveSectionsOneAndTwo(ApplySequence applySequence)
        {
            foreach (var applySection in applySequence.Sections.Where(s => s.SectionNo == 1 || s.SectionNo == 2))
            {
                applySection.NotRequired = true;
                applySection.Status = ApplicationSectionStatus.Evaluated;
            }
        }

        private static bool IsFinancialExempt(ApplyTypes.FHADetails financials, OrganisationType orgType)
        {
            if (financials is null) return false;

            bool financialExempt = financials.FinancialExempt ?? false;
            bool orgTypeFinancialExempt = orgType == null ? false : orgType.FinancialExempt;

            bool financialIsNotDue = (financials.FinancialDueDate?.Date ?? DateTime.MinValue) > DateTime.Today;

            return financialExempt || financialIsNotDue || orgTypeFinancialExempt;
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