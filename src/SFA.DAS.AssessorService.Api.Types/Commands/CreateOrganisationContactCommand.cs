using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class CreateOrganisationContactCommand
    {
      
        public string OrganisationName { get; set; }
        public string OrganisationType { get; set; }
        public string OrganisationUkprn { get; set; }
        public string OrganisationReferenceType { get; set; }
        public bool? IsEpaoApproved { get; set; }
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

        public CreateOrganisationContactCommand(string organisationName, string organisationType, string organisationUkprn, string organisationReferenceType, bool? isEpaoApproved, string tradingName, bool useTradingName, string contactName, string contactAddress1, string contactAddress2, string contactAddress3, string contactAddress4, string contactPostcode, string contactEmail, string contactPhoneNumber, string companyUkprn, string companyNumber, string charityNumber, string standardWebsite)
        {
            OrganisationName = organisationName;
            OrganisationType = organisationType;
            OrganisationUkprn = organisationUkprn;
            OrganisationReferenceType = organisationReferenceType;
            IsEpaoApproved = isEpaoApproved;
            TradingName = tradingName;
            UseTradingName = useTradingName;
            ContactName = contactName;
            ContactAddress1 = contactAddress1;
            ContactAddress2 = contactAddress2;
            ContactAddress3 = contactAddress3;
            ContactAddress4 = contactAddress4;
            ContactPostcode = contactPostcode;
            ContactEmail = contactEmail;
            ContactPhoneNumber = contactPhoneNumber;
            CompanyUkprn = companyUkprn;
            CompanyNumber = companyNumber;
            CharityNumber = charityNumber;
            StandardWebsite = standardWebsite;
        }

        public CreateOrganisationContactCommand()
        {
        }
    }
}
