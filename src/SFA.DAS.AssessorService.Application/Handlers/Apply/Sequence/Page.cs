using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Sequence
{
    public class Page
    {
        public string PageId { get; set; }
        public string Title { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public string QuestionId { get; set; }
        public string Title { get; set; }
        public string Hint { get; set; }
        public Input Input { get; set; }
    }

    public class Input
    {
        public string Type { get; set; }
    }
}