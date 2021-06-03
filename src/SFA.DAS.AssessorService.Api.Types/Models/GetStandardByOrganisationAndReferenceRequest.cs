using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetStandardByOrganisationAndReferenceRequest : IRequest<OrganisationStandard>
    {
        public string OrganisationId { get; set; }

        public string StandardReference { get; set; }
    }
}
