
namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class StandardModel : TestModel
    {
        public string StandardUId { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public decimal Version { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int TypicalDuration { get; set; }
        public int MaxFunding { get; set; }
        public int IsActive { get; set; }
        public int ProposedTypicalDuration { get; set; }
        public int ProposedMaxFunding { get; set; }
    }
}
