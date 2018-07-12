using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities.ao;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationsRequest: IRequest<List<EpaOrganisationType>>
    {
    }
}
