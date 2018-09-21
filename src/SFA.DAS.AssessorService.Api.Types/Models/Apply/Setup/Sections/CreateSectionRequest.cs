using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections
{
    public class CreateSectionRequest : IRequest<Section>
    {
        public string SequenceId { get; }
        public Section Section { get; }

        public CreateSectionRequest(string sequenceId, Section section)
        {
            SequenceId = sequenceId;
            Section = section;
        }
    }
}