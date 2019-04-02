namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{

    using System;

    public class UpdateOrganisationUkprnViewModel
    {
        public string Ukprn { get; set; }
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
