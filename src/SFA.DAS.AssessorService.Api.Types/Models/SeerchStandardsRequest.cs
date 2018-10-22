using System.Collections.Generic;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class SearchStandardsRequest: IRequest<List<StandardSummary>>
    {
        public string Searchstring { get; set; }
    }
}