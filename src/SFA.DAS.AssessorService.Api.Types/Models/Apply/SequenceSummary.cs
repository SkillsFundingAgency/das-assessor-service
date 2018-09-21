namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SequenceSummary
    {
        public SequenceSummary()
        {
            
        }
        
        public SequenceSummary(Sequence sequence)
        {
            SequenceId = sequence.SequenceId;
            Title = sequence.Title;
            LinkTitle = sequence.LinkTitle;
            Active = sequence.Active;
            Complete = sequence.Complete;
            Actor = sequence.Actor;
            Order = sequence.Order.Value;
        }
        
        public string SequenceId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public bool Active { get; set; }
        public bool Complete { get; set; }
        public string Actor { get; set; }
        public int Order { get; set; }
    }
}