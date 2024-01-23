namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi
{
    public class GetAddressesRequest : IGetApiRequest
    {
        public string Query { get; }
        public GetAddressesRequest(string query)
        {
            Query = query;
        }

        public string GetUrl => $"locations?query={Query}";
    }
}