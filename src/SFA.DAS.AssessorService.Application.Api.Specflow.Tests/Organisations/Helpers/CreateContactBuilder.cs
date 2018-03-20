using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers
{
    public class CreateContactBuilder
    {
        public CreateContactRequest Build(dynamic _contactArgument, string endPointAssessorOrganisationId)
        {

            var createContactRequest = new CreateContactRequest
            {
                Username = _contactArgument.UserName,
                DisplayName = _contactArgument.DisplayName,
                Email = _contactArgument.Email,
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId
            };

            return createContactRequest;
        }
    }
}
