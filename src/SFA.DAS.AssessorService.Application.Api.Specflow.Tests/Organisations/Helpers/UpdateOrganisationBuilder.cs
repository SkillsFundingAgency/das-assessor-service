using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers
{
    public class UpdateOrganisationRequestBuilder
    {
        public UpdateOrganisationRequest Build(Organisation organisationl)
        {
            var organisation = new UpdateOrganisationRequest
            {
                PrimaryContact = organisationl.PrimaryContact,
                EndPointAssessorOrganisationId = organisationl.EndPointAssessorOrganisationId,
                EndPointAssessorName = organisationl.EndPointAssessorName

            };

            return organisation;
        }
    }
}
