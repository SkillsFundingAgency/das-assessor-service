namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    // https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-user-entity
    public class Identity
    {
        public string Provider { get; set; }
        public string Id { get; set; }
    }
}
