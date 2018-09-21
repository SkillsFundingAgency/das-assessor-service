using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections
{
    public class DeleteSectionRequest : IRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; }

        public DeleteSectionRequest(string sequenceId, string sectionId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
        }
    }
}