using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections
{
    public class GetSectionRequest : IRequest<Section>
    {
        public string SequenceId { get; }
        public string SectionId { get; set; }

        public GetSectionRequest(string sequenceId, string sectionId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
        }
    }
}