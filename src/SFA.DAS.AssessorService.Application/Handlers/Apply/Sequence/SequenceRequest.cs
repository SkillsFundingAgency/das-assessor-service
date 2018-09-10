using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Sequence
{
    public class SequenceRequest : IRequest<Sequence>
    {
        public int UserId { get; }
        public string SequenceId { get; }

        public SequenceRequest(int userId, string sequenceId)
        {
            UserId = userId;
            SequenceId = sequenceId;
        }
    }
}