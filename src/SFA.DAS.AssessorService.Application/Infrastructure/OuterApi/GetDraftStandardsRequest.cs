
namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetDraftStandardsRequest : IGetApiRequest
    {
        public string GetUrl => $"trainingcourses/draft";
    }
}
