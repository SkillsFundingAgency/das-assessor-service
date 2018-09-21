using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages
{
    public class DeletePageRequest : IRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; }
        public string PageId { get; }

        public DeletePageRequest(string sequenceId, string sectionId, string pageId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}