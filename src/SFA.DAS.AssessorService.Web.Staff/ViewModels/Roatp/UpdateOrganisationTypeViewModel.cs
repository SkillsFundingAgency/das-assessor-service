namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using Api.Types.Models.Roatp;

    public class UpdateOrganisationTypeViewModel
    {
        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }
        public string LegalName { get; set; }
        public Guid OrganisationId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string UpdatedBy { get; set; }
        public int ProviderTypeId { get; set; }
    }
}