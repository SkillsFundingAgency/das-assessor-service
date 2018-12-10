namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GatherStandardsResponse
    {
        public GatherStandardsResponse(string details)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}