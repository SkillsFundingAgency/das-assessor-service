using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class ReturnApplicationRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public int SequenceId { get; }
        public string ReturnType { get; }

        public ReturnApplicationRequest(Guid applicationId, int sequenceId, string returnType)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
            ReturnType = returnType;
        }
    }
}
