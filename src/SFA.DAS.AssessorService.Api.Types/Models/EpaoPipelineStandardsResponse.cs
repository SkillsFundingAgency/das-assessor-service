namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsResponse
    {
        public string StandardName { get; set; }
        public int StandardCode{ get; set; }
        public int Pipeline { get; set; }
        public string EstimatedDate { get; set; }
    }
}
