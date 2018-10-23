using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationContactValidationRequest  : IRequest<ValidationResponse>
    {
        public string ContactId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}