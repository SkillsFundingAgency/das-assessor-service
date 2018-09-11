using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class Question
    {
        public string QuestionId { get; set; }
        public string Title { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public bool Complete { get; set; }
    }
}