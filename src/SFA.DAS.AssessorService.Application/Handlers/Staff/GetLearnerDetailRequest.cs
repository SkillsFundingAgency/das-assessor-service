using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class GetLearnerDetailRequest : IRequest<LearnerDetailResult>
    {
        public int StdCode { get; }
        public long Uln { get; }
        public bool AllRecords { get; }

        public GetLearnerDetailRequest(int stdCode,
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