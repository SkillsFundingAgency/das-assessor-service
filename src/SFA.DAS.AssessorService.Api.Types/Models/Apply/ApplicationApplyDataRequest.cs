using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ApplicationApplyData
    {
        public Guid ApplicationId { get; set; }
        public string ReferenceFormat { get; set; }
        public Guid UserId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public int? StandardCode { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime? LatestStandardSubmissionDate { get; set; }
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }
    }

    public class SubmitApplicationRequest : ApplicationApplyData, IRequest<bool>
    {
        public ApplySequence Sequence { get; set; }
    }
}
