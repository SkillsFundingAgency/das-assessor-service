using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Infrastructure
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByName(string searchTerm, bool exactMatch);
        Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByUkprn(int ukprn);
        Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationInUkrlp(int ukprn);
        Task<OrganisationSearchResult> GetOrganisationByUkprn(long ukprn);
    }
}