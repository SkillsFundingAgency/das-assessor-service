namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SequenceSummary
    {
        public string SequenceId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public bool Active { get; set; }
        public bool Complete { get; set; }
        public string Actor { get; set; }
    }
}