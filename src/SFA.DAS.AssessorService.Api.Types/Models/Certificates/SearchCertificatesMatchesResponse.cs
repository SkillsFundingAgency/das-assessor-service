using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class SearchCertificatesMatchesResponse
    {
        public IEnumerable<SearchCertificatesResponse> Matches { get; set; }
    }
}
