using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class CreateEpaOrganisationValidationRequest : IRequest<ValidationResponse>
    {
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
    }
}
