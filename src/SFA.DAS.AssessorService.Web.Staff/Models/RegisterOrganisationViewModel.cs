using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class RegisterOrganisationViewModel
    {
        public List<OrganisationType> OrganisationTypes { get; set; }

        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string WebsiteLink { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
    }
}
