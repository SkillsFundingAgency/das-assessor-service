namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi
{
    public class GetDraftStandardsRequest : IGetApiRequest
    {
        public string GetUrl => $"trainingcourses/draft";
    }
}
