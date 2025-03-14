using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationRequest, Guid>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;

        public CreateApplicationHandler(IOrganisationQueryRepository organisationQueryRepository,
            IRegisterQueryRepository registerQueryRepository, IContactQueryRepository contactQueryRepository,
            IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _registerQueryRepository = registerQueryRepository;
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<Guid> Handle(CreateApplicationRequest request, CancellationToken cancellationToken)
        {
            var org = await _organisationQueryRepository.Get(request.OrganisationId);
            var orgTypes = await _registerQueryRepository.GetOrganisationTypes();
            var creatingContact = await _contactQueryRepository.GetContactById(request.CreatingContactId);

            if (org != null && orgTypes != null && creatingContact != null)
            {
                var orgType = orgTypes.FirstOrDefault(x => x.Id == org.OrganisationTypeId);

                var sequences = request.ApplySequences;
                RemoveSequencesAndSections(sequences, org, orgType, request.ApplicationType);
              
                var applyData = new Domain.Entities.ApplyData
                {
                    Sequences = sequences,
                    Apply = new Domain.Entities.ApplyInfo
                    {
                        ReferenceNumber = await CreateReferenceNumber(request.ApplicationReferenceFormat)
                    }
                };

                var application = new Domain.Entities.Apply
                {
                    ApplyData = applyData,
                    ApplicationId = request.QnaApplicationId,
                    ApplicationStatus = ApplicationStatus.InProgress,
                    ReviewStatus = ApplicationReviewStatus.Draft,
                    FinancialReviewStatus = Helpers.FinancialReviewStatusHelper.IsFinancialExempt(org.OrganisationData?.FHADetails?.FinancialExempt, org.OrganisationData?.FHADetails.FinancialDueDate, orgType) ? FinancialReviewStatus.Exempt : FinancialReviewStatus.Required,
                    OrganisationId = org.Id,
                    CreatedBy = creatingContact.Id.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                return await _applyRepository.CreateApplication(application);
            }

            return Guid.Empty;
        }

        private void RemoveSequencesAndSections(List<Domain.Entities.ApplySequence> sequences, Domain.Entities.Organisation org, AssessorService.Api.Types.Models.AO.OrganisationType orgType, string applicationType)
        {
            if (applicationType == ApplicationTypes.Initial)
            {
                RemoveSections(sequences, ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO);
                RemoveSections(sequences, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO);
                RemoveSequences(sequences, ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO);

                bool isEpao = IsOrganisationOnEPAORegister(org);
                if (isEpao)
                {
                    RemoveSections(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO, ApplyConst.ORGANISATION_DETAILS_SECTION_NO, ApplyConst.DECLARATIONS_SECTION_NO);
                }

                bool isFinancialExempt = Helpers.FinancialReviewStatusHelper.IsFinancialExempt(org.OrganisationData?.FHADetails.FinancialExempt, org.OrganisationData?.FHADetails.FinancialDueDate, orgType);
                if (isFinancialExempt)
                {
                    RemoveSections(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO, ApplyConst.FINANCIAL_DETAILS_SECTION_NO);
                }

                if (isEpao && isFinancialExempt)
                {
                    RemoveSequences(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO);
                }
            }
            else if (applicationType == ApplicationTypes.OrganisationWithdrawal || applicationType == ApplicationTypes.StandardWithdrawal)
            {
                RemoveSections(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO, ApplyConst.ORGANISATION_DETAILS_SECTION_NO, ApplyConst.DECLARATIONS_SECTION_NO, ApplyConst.FINANCIAL_DETAILS_SECTION_NO);
                RemoveSections(sequences, ApplyConst.STANDARD_SEQUENCE_NO, ApplyConst.STANDARD_DETAILS_SECTION_NO);

                if (applicationType == ApplicationTypes.OrganisationWithdrawal)
                {
                    RemoveSections(sequences, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO);
                    RemoveSequences(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO, ApplyConst.STANDARD_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO);
                }
                else if (applicationType == ApplicationTypes.StandardWithdrawal)
                {
                    RemoveSections(sequences, ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO);
                    RemoveSequences(sequences, ApplyConst.ORGANISATION_SEQUENCE_NO, ApplyConst.STANDARD_SEQUENCE_NO, ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO);
                }
            }

            MakeLowestSequenceActive(sequences);
        }

        private static bool IsOrganisationOnEPAORegister(Domain.Entities.Organisation org)
        {
            if (org?.OrganisationData == null) return false;
            return org.OrganisationData.RoEPAOApproved || org.Status == "Live";
        }

        private async Task<string> CreateReferenceNumber(string referenceFormat)
        {
            var referenceNumber = string.Empty;

            var seq = await _applyRepository.GetNextAppReferenceSequence();

            if (seq > 0 && !string.IsNullOrEmpty(referenceFormat))
            {
                referenceNumber = $"{referenceFormat}{seq:D6}";
            }

            return referenceNumber;
        }

        private void RemoveSequences(List<Domain.Entities.ApplySequence> sequences, params int[] sequenceNumbers)
        {
            foreach (var sequence in sequences.Where(s => sequenceNumbers.Contains(s.SequenceNo)))
            {
                sequence.IsActive = false;
                sequence.NotRequired = true;
                sequence.Status = ApplicationSequenceStatus.Approved;
            }

            // after deactivating all the not required sequences, the next sequence which is 
            // required will be activated and started
            var nextRequiredSequence = sequences.Where(s => !s.NotRequired).OrderBy(s => s.SequenceNo).FirstOrDefault();
            if(nextRequiredSequence != null)
            {
                nextRequiredSequence.IsActive = true;
                nextRequiredSequence.Status = ApplicationSequenceStatus.Draft;
            }
        }

        private void MakeLowestSequenceActive(List<Domain.Entities.ApplySequence> sequences)
        {
            int lowestActiveSequenceNo = sequences.Where(seq => seq.IsActive).Min(seq => seq.SequenceNo);
            foreach (var sequence in sequences.Where(seq => seq.SequenceNo != lowestActiveSequenceNo))
            {
                sequence.IsActive = false;
            }
        }

        public void RemoveSections(List<Domain.Entities.ApplySequence> sequences, int sequenceNumber, params int[] sectionNumbers)
        {
            var sequence = sequences.Single(p => p.SequenceNo == sequenceNumber);
            if (sequence != null)
            {
                foreach (var section in sequence.Sections.Where(s => sectionNumbers.Contains(s.SectionNo)))
                {
                    section.NotRequired = true;
                    section.Status = ApplicationSectionStatus.Evaluated;
                }
            }
        }
    }
}