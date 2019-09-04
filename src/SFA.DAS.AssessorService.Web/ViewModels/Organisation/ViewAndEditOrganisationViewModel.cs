using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;


using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ViewAndEditOrganisationViewModel
    {
        public string OrganisationId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string WebsiteLink { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactName { get; set; }
        public List<ContactResponse> Contacts { get; set; }
        public string ContactsCount { get; set; }
        public bool UserHasChangeOrganisationPrivilege { get; set; }
        public AccessDeniedViewModel AccessDeniedViewModel { get; set; }
        public List<AzureSubscription> ExternalApiSubscriptions { get; set; }
        public string ActionChoice { get; set; }
        public string SubscriptionId { get; set; }
    }
}
 