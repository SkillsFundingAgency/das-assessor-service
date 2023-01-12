using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
    public class ApplicationTypeResolver : IValueResolver<ApplySummary, ApplicationResponse, string>
    {
        public string Resolve(ApplySummary source, ApplicationResponse destination, string destMember, ResolutionContext context)
        {
            bool OrganisationSectionRequired(int sectionNo) =>
                IsSectionRequired(source.ApplyData, ApplyConst.ORGANISATION_SEQUENCE_NO, sectionNo);

            if (IsSequenceRequired(source.ApplyData, ApplyConst.ORGANISATION_SEQUENCE_NO) &&
                OrganisationSectionRequired(ApplyConst.ORGANISATION_DETAILS_SECTION_NO) &&
                OrganisationSectionRequired(ApplyConst.DECLARATIONS_SECTION_NO))
            {
                if (OrganisationSectionRequired(ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                {
                    return ApplicationTypes.InitialWithFinancialHealthChecks;
                }
                else return ApplicationTypes.InitialWithoutFinancialHealthChecks;
            }
            else if (IsSequenceRequired(source.ApplyData, ApplyConst.STANDARD_SEQUENCE_NO))
            {
                if (OrganisationSectionRequired(ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                {
                    return ApplicationTypes.AdditionalStandardWithFinancialHealthChecks;
                }
                else return ApplicationTypes.AdditionalStandardWithoutFinancialHealthChecks;
            }
            else if (IsSequenceRequired(source.ApplyData, ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO))
            {
                return ApplicationTypes.OrganisationWithdrawal;
            }
            else if (IsSequenceRequired(source.ApplyData, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO))
            {
                return ApplicationTypes.StandardWithdrawal;
            }

            return string.Empty;
        }

        private bool IsSequenceRequired(ApplyData applyData, int sequenceNo)
        {
            // a sequence cannot be considered required if it does not exist in the ApplyData
            return applyData
                ?.Sequences
                ?.Any(x => x.SequenceNo == sequenceNo && !x.NotRequired) ?? false;
        }

        private bool IsSectionRequired(ApplyData applyData, int sequenceNo, int sectionNo)
        {
            // a sequence section cannot be considered required if the sequence or sequence section does not exist in the ApplyData
            return applyData
                ?.Sequences
                ?.Where(sequence => sequence.SequenceNo == sequenceNo && !sequence.NotRequired)
                ?.SelectMany(sequence => sequence.Sections.Where(section => section.SectionNo == sectionNo && !section.NotRequired))
                ?.Any() ?? false;
        }
    }
}
