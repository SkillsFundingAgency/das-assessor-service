using System.Collections.Generic;
using System.Reflection;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class Page
    {
        public string PageId { get; set; }
        public string SequenceId { get; set; }
        public string Title { get; set; }
        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public Next Next { get; set; }
        public bool Complete { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public int? Order { get; set; }
    }

    public class UpdatePageResult
    {
        public Page Page { get; set; }
        public bool ValidationPassed { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
    }
}