using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class UpdateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }       
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
        public int EndPointAssessorUkprn { get; set; }
    }
}
