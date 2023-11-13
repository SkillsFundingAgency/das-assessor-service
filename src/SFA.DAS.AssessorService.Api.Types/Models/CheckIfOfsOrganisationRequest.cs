using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public record CheckIfOfsOrganisationRequest(int Ukprn) : IRequest<bool>;
}
