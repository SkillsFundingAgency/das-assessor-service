
namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetActiveStandardsRequest : IGetApiRequest
    {
        public string GetUrl => $"trainingcourses/active";
    }
}
