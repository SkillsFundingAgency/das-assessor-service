namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;

    public class AddOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public string LegalName { get; set; }
        public string UKPRN { get; set; }
        public int OrganisationTypeId { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string TradingName { get; set; }
        public DateTime? ApplicationDeterminedDate { get; set; }
        public IEnumerable<ProviderType> ProviderTypes { get; set; }
        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }

    }

    public class AddOrganisationProviderTypeViewModel : AddOrganisationViewModel
    {
        public AddOrganisationProviderTypeViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }

    }

    public class AddOrganisationTypeViewModel : AddOrganisationViewModel
    {
        public AddOrganisationTypeViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }
    }

    public class AddApplicationDeterminedDateViewModel : AddOrganisationViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }


        public AddApplicationDeterminedDateViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }
    }

    public class AddOrganisationViaUkprnViewModel : AddOrganisationViewModel
    {
        public AddOrganisationViaUkprnViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }
    }
}
