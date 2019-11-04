using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class FeedbackAddedApplicationsRequest : IRequest<List<ApplicationSummaryItem>>
    {
        public int SequenceNo { get; }

        public FeedbackAddedApplicationsRequest(int sequenceNo)
        {
            SequenceNo = sequenceNo;
        }
    }
}
