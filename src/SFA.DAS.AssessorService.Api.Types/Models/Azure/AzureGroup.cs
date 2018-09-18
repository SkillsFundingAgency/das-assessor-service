namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;

    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Group
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-group-entity
    public class AzureGroup
    {
        [JsonProperty("id")]
        public string AzureId { get; set; }
        [JsonIgnore]
        public string Id => AzureId?.Replace("/groups/", string.Empty);
        public string Name { get; set; }
        public string Description { get; set; }
        public bool BuiltIn { get; set; }
        public string Type { get; set; }
        public string ExternalId { get; set; }
    }
}
