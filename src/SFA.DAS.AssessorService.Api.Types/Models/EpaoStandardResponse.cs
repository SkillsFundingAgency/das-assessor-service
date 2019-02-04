namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoStandardResponse
    {
        public EpaoStandardResponse(string details)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}