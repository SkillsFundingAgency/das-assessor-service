using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages
{
    public class CreatePageRequest : IRequest<Page>
    {
        public string SequenceId { get; }
        public string SectionId { get; }
        public Page Page { get; }

        public CreatePageRequest(string sequenceId, string sectionId, Page page)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            Page = page;
        }
    }
}