using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class UpdateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
        public long? EndPointAssessorUkprn { get; set; }
        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }
    }
}
