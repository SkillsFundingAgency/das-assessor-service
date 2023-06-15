using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    /// <summary>
    /// Searches for the organisation
    /// <param name="epaoId">The epao identifier.</param>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="ukprn">The ukprn.</param>
    /// <param name="organisations">The list of organisations.</param>
    /// </summary>
    public interface IOrganisationSearchOrchestrator
    {
        /// <summary>Organisations the search by epao.</summary>

        /// <returns>
        ///   <br />
        /// </returns>
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByEpao(string epaoId);
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByNameOrCharityNumberOrCompanyNumber(string searchTerm);
        Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByUkprn(int ukprn);
        IEnumerable<OrganisationSearchResult> Dedupe(IEnumerable<OrganisationSearchResult> organisations);
    }
}