using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class Section
    {
        public string SectionId { get; set; }
        public string Title { get; set; }
        public List<Page> Pages { get; set; }
    }
}