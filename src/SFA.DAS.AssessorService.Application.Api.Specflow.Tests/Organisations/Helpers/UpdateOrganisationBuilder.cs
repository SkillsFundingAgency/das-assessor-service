using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers
{
    public class UpdateOrganisationRequestBuilder
    {
        public UpdateOrganisationRequest Build(Organisation organisation)
        {
            var updateOrganisation = new UpdateOrganisationRequest
            {
                PrimaryContact = organisation.PrimaryContact,
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                EndPointAssessorName = organisation.EndPointAssessorName,
                EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                ApiEnabled = organisation.ApiEnabled,
                ApiUser = organisation.ApiUser
            };

            return updateOrganisation;
        }
    }
}
