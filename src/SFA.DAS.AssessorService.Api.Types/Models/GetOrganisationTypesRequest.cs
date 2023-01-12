using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationTypesRequest : IRequest<List<AO.OrganisationType>>
    {
    }
}
