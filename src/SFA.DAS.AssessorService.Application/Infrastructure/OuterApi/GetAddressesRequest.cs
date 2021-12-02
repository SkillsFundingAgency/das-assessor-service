namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
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