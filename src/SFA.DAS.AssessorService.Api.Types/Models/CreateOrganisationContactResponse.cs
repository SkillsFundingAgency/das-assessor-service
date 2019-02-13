using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationContactResponse
    {
        public CreateOrganisationContactResponse()
        {
        }

        public CreateOrganisationContactResponse(string organisationId, bool organisationAdded, bool contactAdded,  List<string> warningMessages )
        {
            OrganisationId = organisationId;
            WarningMessages = warningMessages;
            ContactAdded = contactAdded;
            OrganisationAdded = organisationAdded;
        }

        public string OrganisationId { get; set; }
        public List<string> WarningMessages { get; set; }
        public bool OrganisationAdded { get; set; }
        public bool ContactAdded { get; set; }
    }
}
