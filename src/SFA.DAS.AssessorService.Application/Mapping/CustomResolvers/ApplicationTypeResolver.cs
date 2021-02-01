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
            if (IsSequenceRequired(source.ApplyData, ApplyConst.ORGANISATION_SEQUENCE_NO) && IsSequenceRequired(source.ApplyData, ApplyConst.STANDARD_SEQUENCE_NO))
            {
                return ApplicationTypes.Combined;
            }
            else if (IsSequenceRequired(source.ApplyData, ApplyConst.STANDARD_SEQUENCE_NO))
            {
                return ApplicationTypes.Standard;
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
            return applyData?.Sequences?.Any(x => x.SequenceNo == sequenceNo && !x.NotRequired) ?? false;
        }
    }
}
