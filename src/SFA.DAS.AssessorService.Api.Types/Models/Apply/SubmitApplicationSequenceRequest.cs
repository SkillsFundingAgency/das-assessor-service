using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SubmitApplicationSequenceRequest : IRequest<bool>
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationReferenceFormat { get; set; }
        public int SequenceNo { get; set; }  
        public Guid SubmittingContactId { get; set; }
    }
}
