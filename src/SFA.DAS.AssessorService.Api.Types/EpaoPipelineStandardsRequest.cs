using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class EpaoPipelineStandardsRequest: IRequest<PaginatedList<EpaoPipelineStandardsResponse>>
    {
        public EpaoPipelineStandardsRequest(string epaoId, string orderBy, string orderDirection, int? pageIndex, int pageSize)
        {
            EpaoId = epaoId;
            PageIndex = pageIndex;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            PageSize = pageSize;
        }
        public int PageSize { get; set; }
        public int? PageIndex { get; private set; }
        public string EpaoId { get; private set; }

        public string OrderBy { get; private set; }
        public string OrderDirection { get; private set; }



    }
}
