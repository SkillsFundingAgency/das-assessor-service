namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;
    using System;

    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Subscription
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-subscription-entity
    public class AzureSubscription
    {
        [JsonProperty("id")]
        public string AzureId { get; set; }
        [JsonProperty("userId")]
        public string AzureUserId { get; set; }
        [JsonProperty("productId")]
        public string AzureProductId { get; set; }

        [JsonIgnore]
        public string Id => AzureId?.Replace("/subscriptions/", string.Empty);
        [JsonIgnore]
        public string UserId => AzureUserId?.Replace("/users/", string.Empty);
        [JsonIgnore]
        public string ProductId => AzureProductId?.Replace("/products/", string.Empty);
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NotificationDate { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string StateComment { get; set; }
    }
}
