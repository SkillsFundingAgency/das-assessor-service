using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    public interface IReferenceDataApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiation(string searchTerm, bool exactMatch);
    }
}
