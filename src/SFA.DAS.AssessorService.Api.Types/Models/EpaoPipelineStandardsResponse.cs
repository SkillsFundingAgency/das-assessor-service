namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsResponse
    {
        public string StandardName { get; set; }
        public string StandardReference { get; set; }
        public string StandardVersion { get; set; }
        public string UKPRN { get; set; }
        public string TrainingProvider { get; set; }
        public int Pipeline { get; set; }
        public string EstimatedDate { get; set; }
    }
}
