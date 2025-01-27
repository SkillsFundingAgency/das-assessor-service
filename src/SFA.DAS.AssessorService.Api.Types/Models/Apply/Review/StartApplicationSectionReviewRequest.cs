using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class StartApplicationSectionReviewRequest : IRequest<Unit>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string Reviewer { get; }

        public StartApplicationSectionReviewRequest(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            Reviewer = reviewer;
        }
    }
}