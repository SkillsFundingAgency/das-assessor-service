using System.Collections.Generic;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchStandardsRequest: IRequest<List<StandardCollation>>
    {
        public string SearchTerm { get; set; }
    }
}