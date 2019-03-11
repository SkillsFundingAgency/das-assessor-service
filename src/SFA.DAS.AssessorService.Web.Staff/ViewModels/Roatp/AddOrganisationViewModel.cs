namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;

    public class AddOrganisationViewModel
    {
        public AddOrganisationViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }

        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public string LegalName { get; set; }
        public string UKPRN { get; set; }
        public int OrganisationTypeId { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string TradingName { get; set; }

        public IEnumerable<ProviderType> ProviderTypes { get; set; }
        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }

        public List<string> ValidationErrors { get; set; }

    }
}
