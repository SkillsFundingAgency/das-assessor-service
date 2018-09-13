using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class Sequence
    {
        public string SequenceId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public bool Active { get; set; }
        public bool Complete { get; set; }
        public List<Section> Sections { get; set; }
        public NextSequence NextSequence { get; set; }
        public string Actor { get; set; }
    }

    public class NextSequence
    {
        public string NextSequenceId { get; set; }
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        public string QuestionId { get; set; }
        public string MustEqual { get; set; }
    }
}