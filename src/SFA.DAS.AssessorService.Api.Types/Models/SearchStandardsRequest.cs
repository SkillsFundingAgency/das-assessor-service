using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchStandardsRequest : IRequest<List<Standard>>
    {
        public string SearchTerm { get; set; }
    }
}