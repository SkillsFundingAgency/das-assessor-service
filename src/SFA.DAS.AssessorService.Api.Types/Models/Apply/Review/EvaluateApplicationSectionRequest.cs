using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class EvaluateApplicationSectionRequest : IRequest<Unit>
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public bool IsSectionComplete { get; }
        public string EvaluatedBy { get; }

        public EvaluateApplicationSectionRequest(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            IsSectionComplete = isSectionComplete;
            EvaluatedBy = evaluatedBy;
        }
    }
}
