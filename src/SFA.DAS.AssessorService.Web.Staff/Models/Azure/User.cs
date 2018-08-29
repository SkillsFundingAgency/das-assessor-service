using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#User
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-user-entity
    public class User
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string AzureId { get; set; }

        public string Id => AzureId?.Replace("/users/", string.Empty);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Note { get; set; }
        public List<Identity> Identities { get; set; } = new List<Identity>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
