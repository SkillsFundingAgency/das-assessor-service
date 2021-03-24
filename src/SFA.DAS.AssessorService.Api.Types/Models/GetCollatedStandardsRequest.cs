using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetCollatedStandardsRequest : IRequest<List<StandardCollation>>
    {
    }
}
