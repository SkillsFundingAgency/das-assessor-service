using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Auditing
{
    public class EpaOrganisationAuditFilter : IAuditFilter
    {
        public string FilterAuditDiff(string propertyChanged)
        {
            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.PhoneNumber)}")
            {
                return "Contact phone number";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Email)}")
            {
                return "Email address";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.WebsiteLink)}")
            {
                return "Website address";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Address1)}")
            {
                return "Contact address line 1";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Address2)}")
            {
                return "Contact address line 2";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Address3)}")
            {
                return "Contact address line 3";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Address4)}")
            {
                return "Contact address line 4";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.OrganisationData)}.{nameof(OrganisationData.Postcode)}")
            {
                return "Contact address postcode";
            }

            if (propertyChanged == $"{nameof(EpaOrganisation.PrimaryContact)}")
            {
                return "Contact name";
            }

            return null;
        }
    }
}
