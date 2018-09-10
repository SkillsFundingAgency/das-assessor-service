using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Sequence
{
    public class Section
    {
        public string SectionId { get; set; }
        public string Title { get; set; }
        public List<Page> Pages { get; set; }
    }
}