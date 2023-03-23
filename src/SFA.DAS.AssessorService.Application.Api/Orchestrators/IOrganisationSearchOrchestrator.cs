using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    public interface IOrganisationSearchOrchestrator
    {
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByEpao(string epaoId);
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByNameOrCharityNumberOrCompanyNumber(string searchTerm);
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByUkprn(int ukprn);
        IEnumerable<OrganisationSearchResult> Dedupe(IEnumerable<OrganisationSearchResult> organisations);
    }
}