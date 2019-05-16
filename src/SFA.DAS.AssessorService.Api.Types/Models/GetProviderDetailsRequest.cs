using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetProviderDetailsRequest : IRequest<List<ProviderDetails>>
    {
        public string Ukprn { get; set; }
    }
}
