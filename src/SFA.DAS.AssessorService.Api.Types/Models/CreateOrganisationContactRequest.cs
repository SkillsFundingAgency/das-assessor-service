using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationContactRequest: IRequest<CreateOrganisationContactResponse>
    {
        public string OrganisationName { get; set; }
        public string OrganisationType { get; set; }
        public string OrganisationUkprn { get; set; }
        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress1 { get; set; }
        public string ContactAddress2 { get; set; }
        public string ContactAddress3 { get; set; }
        public string ContactAddress4 { get; set; }

        public string ContactPostcode { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string CompanyUkprn { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string StandardWebsite { get; set; }
    }
}
