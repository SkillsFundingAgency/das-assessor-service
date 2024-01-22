using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByUkprn(int ukprn);
        Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationInUkrlp(int ukprn);
        Task<OrganisationSearchResult> GetOrganisationByUkprn(long ukprn);
    }
}