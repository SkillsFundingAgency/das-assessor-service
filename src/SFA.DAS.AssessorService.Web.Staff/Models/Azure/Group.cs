namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-contract-reference#Group
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-group-entity
    public class Group
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string AzureId { get; set; }

        public string Id => AzureId?.Replace("/groups/", string.Empty);
        public string Name { get; set; }
        public string Description { get; set; }
        public bool BuiltIn { get; set; }
        public string Type { get; set; }
        public string ExternalId { get; set; }
    }
}
