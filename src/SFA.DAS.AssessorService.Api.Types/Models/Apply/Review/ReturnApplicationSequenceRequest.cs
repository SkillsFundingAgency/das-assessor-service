using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class ReturnApplicationSequenceRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public int SequenceId { get; }
        public string ReturnType { get; }
        public string ReturnedBy { get; }

        public ReturnApplicationSequenceRequest(Guid applicationId, int sequenceId, string returnType, string returnedBy)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
            ReturnType = returnType;
            ReturnedBy = returnedBy;
        }
    }
}
