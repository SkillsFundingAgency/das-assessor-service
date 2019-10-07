using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class StartApplicationSequenceReviewRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public string StartedBy { get; set; }

        public StartApplicationSequenceReviewRequest(Guid applicationId, int sequenceNo, string startedBy)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            StartedBy = startedBy;
        }
    }
}