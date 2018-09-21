using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences
{
    public class DeleteSequenceRequest : IRequest
    {
        public string SequenceId { get; }

        public DeleteSequenceRequest(string sequenceId)
        {
            SequenceId = sequenceId;
        }
    }
}