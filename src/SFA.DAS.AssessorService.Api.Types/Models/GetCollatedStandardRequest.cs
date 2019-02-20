using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetCollatedStandardRequest : IRequest<StandardCollation>
    {
        public int StandardId;
    }
}
