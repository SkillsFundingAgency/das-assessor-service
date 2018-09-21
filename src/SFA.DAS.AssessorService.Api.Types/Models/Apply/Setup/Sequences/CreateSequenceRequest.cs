using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences
{
    public class CreateSequenceRequest : IRequest<Sequence>
    {
        public Sequence Sequence { get; }

        public CreateSequenceRequest(Sequence sequence)
        {
            Sequence = sequence;
        }
    }
}