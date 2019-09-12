namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OppFinderApprovedStandard
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public string StandardReference { get; set; }
        public int ActiveApprentices { get; set; }
        public int RegisteredEPAOs { get; set; }
    }
}
