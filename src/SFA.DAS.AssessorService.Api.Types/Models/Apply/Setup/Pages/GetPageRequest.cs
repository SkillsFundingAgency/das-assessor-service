using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages
{
    public class GetPageRequest : IRequest<Page>
    {
        public string SequenceId { get; }
        public string SectionId { get; set; }
        public string PageId { get; }

        public GetPageRequest(string sequenceId, string sectionId, string pageId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}