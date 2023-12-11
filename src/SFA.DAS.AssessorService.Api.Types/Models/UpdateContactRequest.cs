namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class UpdateContactRequest : IRequest
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
