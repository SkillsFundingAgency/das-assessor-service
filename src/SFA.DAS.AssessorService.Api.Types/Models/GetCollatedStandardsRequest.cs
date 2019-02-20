using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetCollatedStandardsRequest : IRequest<List<StandardCollation>>
    {
    }
}
