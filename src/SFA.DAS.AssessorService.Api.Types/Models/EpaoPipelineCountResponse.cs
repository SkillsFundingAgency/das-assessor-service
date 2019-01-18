namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineCountResponse
    {
        public EpaoPipelineCountResponse(int count)
        {
            Count = count;
        }

        public int Count { get; set; }
    }
}