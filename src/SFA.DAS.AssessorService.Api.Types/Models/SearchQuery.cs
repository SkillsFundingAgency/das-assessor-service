using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchQuery : IRequest<List<SearchResult>>
    {
        public long Uln { get; set; }
        public string Surname { get; set; }
        public string EpaOrgId { get; set; }
        public string Username { get; set; }
    }
}