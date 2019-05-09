namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsExtractResponse
    {
        public string StandardName { get; set; }
        public int Pipeline { get; set; }
        public int ProviderUkPrn { get; set; }
        public string EstimatedDate { get; set; }
    }
}
