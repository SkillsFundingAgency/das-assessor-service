namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#User
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-user-entity
    public class AzureUser
    {
        [JsonProperty("id")]
        public string AzureId { get; set; }
        [JsonIgnore]
        public string Id => AzureId?.Replace("/users/", string.Empty);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Note { get; set; }
        [JsonIgnore]
        public string Ukprn => Note?.Replace("ukprn=", string.Empty);
        public List<AzureIdentity> Identities { get; set; } = new List<AzureIdentity>();
        public List<AzureGroup> Groups { get; set; } = new List<AzureGroup>();
        public List<AzureSubscription> Subscriptions { get; set; } = new List<AzureSubscription>();
    }
}
