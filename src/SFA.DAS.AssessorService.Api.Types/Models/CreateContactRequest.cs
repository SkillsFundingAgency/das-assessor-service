namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class CreateContactRequest : IRequest<ContactResponse>
    {
        public string EndPointAssessorOrganisationId { get; set; }

        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
