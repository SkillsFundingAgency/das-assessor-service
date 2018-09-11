using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class Page
    {
        public string PageId { get; set; }
        public string Title { get; set; }
        public List<Question> Questions { get; set; }
        public Next Next { get; set; }
        public bool Complete { get; set; }
    }
}