namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;

    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Product
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-product-entity
    public class AzureProduct
    {
        [JsonProperty("id")]
        public string AzureId { get; set; }
        [JsonIgnore]
        public string Id => AzureId?.Replace("/products/", string.Empty);
        public string Name { get; set; }
        public string Description { get; set; }
        public string Terms { get; set; }
        public bool SubscriptionRequired { get; set; }
        public bool ApprovalRequired { get; set; }
        public int SubscriptionsLimit { get; set; }
        public string State { get; set; }
    }
}
