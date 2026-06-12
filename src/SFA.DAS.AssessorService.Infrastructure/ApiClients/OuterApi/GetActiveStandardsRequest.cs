namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi
{
    public class GetActiveStandardsRequest : IGetApiRequest
    {
        public string GetUrl => $"trainingcourses/active";
    }
}
