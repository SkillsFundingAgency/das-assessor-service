using System.Collections.Generic;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchStandardsRequest: IRequest<List<StandardSummary>>
    {
        public string SearchTerm { get; set; }
    }
}