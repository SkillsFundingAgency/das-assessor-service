using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class CreateEpaOrganisationContactRequest : IRequest<string>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}