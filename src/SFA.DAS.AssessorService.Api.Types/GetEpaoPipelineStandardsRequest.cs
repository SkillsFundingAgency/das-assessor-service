using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class GetEpaoPipelineStandardsRequest: IRequest<PaginatedList<GetEpaoPipelineStandardsResponse>>
    {
        public GetEpaoPipelineStandardsRequest(string epaoId, int? pageIndex)
        {
            EpaoId = epaoId;
            PageIndex = pageIndex;
        }

        public int? PageIndex { get; private set; }
        public string EpaoId { get; private set; }
    }
}
