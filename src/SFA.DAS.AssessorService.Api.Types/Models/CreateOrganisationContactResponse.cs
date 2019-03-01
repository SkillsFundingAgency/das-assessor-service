using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationContactResponse
    {
        public CreateOrganisationContactResponse()
        {
        }

        public CreateOrganisationContactResponse(string organisationId,  List<string> warningMessages )
        {
            OrganisationId = organisationId;
            WarningMessages = warningMessages;
        }

        public string OrganisationId { get; set; }
        public List<string> WarningMessages { get; set; }
    }
}
