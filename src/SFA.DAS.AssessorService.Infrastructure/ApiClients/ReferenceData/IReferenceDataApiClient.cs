using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData
{
    public interface IReferenceDataApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiation(string searchTerm, bool exactMatch);
    }
}
