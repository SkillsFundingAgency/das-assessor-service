using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    /// <summary>
    /// Organisation search orchestrator
    /// </summary>
    public interface IOrganisationSearchOrchestrator
    {
        /// <summary>Organisations search by epao.</summary>
        /// <param name="epaoId">The epao identifier.</param>
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByEpao(string epaoId);
        /// <summary>Organisations search by name, charity number or company number.</summary>
        /// <param name="searchTerm">The search term.</param>
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByNameOrCharityNumberOrCompanyNumber(string searchTerm);
        /// <summary>Organisations search by ukprn.</summary>
        /// <param name="ukprn">The ukprn.</param>
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByUkprn(int ukprn);
        /// <summary>Dedupes the specified organisations.</summary>
        /// <param name="organisations">The organisations.</param>
        IEnumerable<OrganisationSearchResult> Dedupe(IEnumerable<OrganisationSearchResult> organisations);
    }
}