namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup
{
    public class SectionSummary
    {
        public SectionSummary()
        {
            
        }
        
        public SectionSummary(Section section)
        {
            SectionId = section.SectionId;
            Title = section.Title;
            Order = section.Order.Value;
        }
        
        public string SectionId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }
}