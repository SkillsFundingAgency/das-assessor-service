namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;

    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Api
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-api-entity
    public class AzureApi
    {
        [JsonProperty("id")]
        public string AzureId { get; set; }
        [JsonIgnore]
        public string Id => AzureId?.Replace("/apis/", string.Empty);
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceUrl { get; set; }
        public string Path { get; set; }
    }
}
