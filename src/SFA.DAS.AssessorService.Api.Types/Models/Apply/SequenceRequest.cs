using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SequenceRequest : IRequest<Sequence>
    {
        public string UserId { get; }
        public string SequenceId { get; }

        public SequenceRequest(string userId, string sequenceId)
        {
            UserId = userId;
            SequenceId = sequenceId;
        }
    }
}