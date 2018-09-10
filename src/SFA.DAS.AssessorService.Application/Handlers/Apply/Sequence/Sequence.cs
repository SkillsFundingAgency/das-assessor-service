using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Sequence
{
    public class Sequence
    {
        public string SequenceId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public bool Active { get; set; }
        public bool Completed { get; set; }
        public List<Section> Sections { get; set; }
    }
}