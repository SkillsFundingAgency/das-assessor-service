using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SubmitApplicationSequenceRequest : IRequest<bool>
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationReferenceFormat { get; set; }
        public int SequenceNo { get; set; }  
        public Guid SubmittingContactId { get; set; }
        public Dictionary<int, bool?>  RequestedFeedbackAnswered { get; set; }
    }
}
