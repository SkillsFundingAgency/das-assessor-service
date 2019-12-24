using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationTypesRequest: IRequest<List<AO.OrganisationType>>
    {
    }
}
