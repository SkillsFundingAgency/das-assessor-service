﻿using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaoRegisteredStandardsRequest : IRequest<PaginatedList<GetEpaoRegisteredStandardsResponse>>
    {
        public GetEpaoRegisteredStandardsRequest(string epaoId, bool requireAtLeastOneVersion, int pageIndex, int pageSize)
        {
            EpaoId = epaoId;
            RequireAtLeastOneVersion = requireAtLeastOneVersion;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageIndex { get; private set; }
        public bool RequireAtLeastOneVersion { get; private set; }
        public int PageSize { get; private set; }
        public string EpaoId { get; private set; }
    }
}
