using System;

namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Subscription
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-subscription-entity
    public class Subscription
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string AzureId { get; set; }
        [Newtonsoft.Json.JsonProperty("userId")]
        public string AzureUserId { get; set; }
        [Newtonsoft.Json.JsonProperty("productId")]
        public string AzureProductId { get; set; }

        public string Id => AzureId?.Replace("/subscriptions/", string.Empty);
        public string UserId => AzureUserId?.Replace("/users/", string.Empty);
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
