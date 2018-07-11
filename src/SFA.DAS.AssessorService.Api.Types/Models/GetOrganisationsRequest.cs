using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationsRequest: IRequest<List<OrganisationTypeResponse>>
    {
    }
}
