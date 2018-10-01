using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class CreateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public long? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
    }
}
