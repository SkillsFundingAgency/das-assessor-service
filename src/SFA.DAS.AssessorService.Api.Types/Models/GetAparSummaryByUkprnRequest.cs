using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAparSummaryByUkprnRequest : IRequest<List<AparSummary>>
    {
        public int Ukprn { get; }

        public GetAparSummaryByUkprnRequest(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
