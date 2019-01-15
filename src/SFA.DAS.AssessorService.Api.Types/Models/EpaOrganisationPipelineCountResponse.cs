namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationPipelineCountResponse
    {
        public EpaOrganisationPipelineCountResponse(int count)
        {
            Count = count;
        }

        public int Count { get; set; }
    }
}