using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class CreateContactRequest : IRequest<ContactBoolResponse>
    {
        public CreateContactRequest()
        {
        }

        public CreateContactRequest(string givenName, string familyName, string email, string epaOrg, string userName, string govIdentifier)
        {
            GivenName = givenName;
            FamilyName = familyName;
            Email = email;
            EndPointAssessorOrganisationId = epaOrg;
            Username = userName;
            DisplayName = $"{givenName} {familyName}";
            GovIdentifier = govIdentifier;
        }
       

        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string GovIdentifier { get; set; }
    }
}
