using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class EvaluateSectionRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public bool IsSectionComplete { get; }
        public string EvaluatedBy { get; set; }

        public EvaluateSectionRequest(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            IsSectionComplete = isSectionComplete;
            EvaluatedBy = evaluatedBy;
        }
    }
}
