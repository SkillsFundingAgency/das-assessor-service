using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup
{
    public class GetSequenceRequest : IRequest<Sequence>
    {
        public string SequenceId { get; set; }

        public GetSequenceRequest(string sequenceId)
        {
            SequenceId = sequenceId;
        }
    }
}