using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class StartApplicationReviewRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }

        public StartApplicationReviewRequest(Guid applicationId, int sequenceNo)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
        }
    }
}
}
