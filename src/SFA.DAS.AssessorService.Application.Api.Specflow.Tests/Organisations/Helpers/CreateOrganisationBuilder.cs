using System;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers
{
    public class CreateOrganisationBuilder
    {
        public CreateOrganisationRequest Build(dynamic _organisationArgument,string primaryContact = null)
        {
            var organisation = new CreateOrganisationRequest
            {
                EndPointAssessorName = _organisationArgument.EndPointAssessorName,
                EndPointAssessorOrganisationId = _organisationArgument.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUkprn = Convert.ToInt32(_organisationArgument.EndPointAssessorUKPRN),
                PrimaryContact = primaryContact
            };

            return organisation;
        }
    }
}
