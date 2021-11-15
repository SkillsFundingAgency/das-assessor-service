using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;

namespace SFA.DAS.AssessorService.Application.Handlers.Approvals
{
    public class GetApprovalsLearnerRecordRequest : IRequest<ApprovalsLearnerResult>
    {
        public int StdCode { get; }
        public long Uln { get; }
        
        public GetApprovalsLearnerRecordRequest(int stdCode, long uln)
        {
            StdCode = stdCode;
            Uln = uln;
        }
    }
}