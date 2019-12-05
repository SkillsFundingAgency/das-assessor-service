using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class LearnerDetailForStaffRequest : IRequest<LearnerDetailForStaff>
    {
        public int StdCode { get; }
        public long Uln { get; }
        public bool AllRecords { get; }

        public LearnerDetailForStaffRequest(int stdCode,
            long uln,
            bool allRecords
            )
        {
            StdCode = stdCode;
            Uln = uln;
            AllRecords = allRecords;
        }
    }
}