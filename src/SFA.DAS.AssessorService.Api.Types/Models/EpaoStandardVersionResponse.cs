
namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoStandardVersionResponse
    {
        public string Details { get; set; }

        public EpaoStandardVersionResponse(string details)
        {
            Details = details;
        }
    }
}
