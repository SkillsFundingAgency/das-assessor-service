namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Product
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-product-entity
    public class Product
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string AzureId { get; set; }

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
