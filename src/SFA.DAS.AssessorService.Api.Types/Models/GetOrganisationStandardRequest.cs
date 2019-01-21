using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationStandardRequest: IRequest<OrganisationStandard>
    {
        public int OrganisationStandardId;    
    }
}