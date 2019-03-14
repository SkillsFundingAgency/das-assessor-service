using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class CreateContactRequest : IRequest<ContactBoolResponse>
    {
        public CreateContactRequest(string givenName, string familyName, string email, string epaOrg, string userName)
        {
            GivenName = givenName;
            FamilyName = familyName;
            Email = email;
            EndPointAssessorOrganisationId = epaOrg;
            Username = userName;
            DisplayName = $"{givenName} {familyName}";
        }
       

        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FamilyName { get; }
        public string GivenName { get; }
    }
}
