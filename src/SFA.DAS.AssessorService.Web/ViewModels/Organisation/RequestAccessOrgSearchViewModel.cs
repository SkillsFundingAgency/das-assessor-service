using AutoMapper;

namespace SFA.DAS.AssessorService.Web.ViewModels.Organisation
{
    public class RequestAccessOrgSearchViewModel : OrganisationSearchViewModel
    {
        public string Address { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyOrCharityDisplayText { get; set; }
        public bool RoEPAOApproved { get; set; }
        public bool OrganisationIsLive { get; set; }
    }
}
