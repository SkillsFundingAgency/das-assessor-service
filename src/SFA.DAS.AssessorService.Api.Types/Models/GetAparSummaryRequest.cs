using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAparSummaryRequest : IRequest<List<AparSummary>>
    {
        public int? Ukprn { get; }

        public GetAparSummaryRequest()
        {
        }

        public GetAparSummaryRequest(int? ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
