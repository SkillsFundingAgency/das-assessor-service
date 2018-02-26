using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.Types;

namespace SFA.DAS.AssessorService.ExternalApis
{
    public interface IAssessmentOrgsApiClient : IDisposable
    {
        /// <summary>
        /// Get a single organisation details
        /// GET /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a organisation details based on id</returns>
        Organisation Get(string organisationId);

        /// <summary>
        /// Get a single organisation details
        /// GET /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a organisation details based on id</returns>
        Task<Organisation> GetAsync(string organisationId);

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">an integer for the standard id</param>
        /// <returns>a collection of organisation</returns>
        IEnumerable<Organisation> ByStandard(int standardId);

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">an integer for the standard id</param>
        /// <returns>a collection of organisation</returns>
        Task<IEnumerable<Organisation>> ByStandardAsync(int standardId);

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">a string for the standard id</param>
        /// <returns>a collection of organisation</returns>
        IEnumerable<Organisation> ByStandard(string standardId);

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">a string for the standard id</param>
        /// <returns>a collection of organisation</returns>
        Task<IEnumerable<Organisation>> ByStandardAsync(string standardId);

        /// <summary>
        /// Get a collection of organisations
        /// GET /frameworks
        /// </summary>
        /// <returns>a collection of organisation summaries</returns>
        IEnumerable<OrganisationSummary> FindAll();

        /// <summary>
        /// Get a collection of organisations
        /// GET /frameworks
        /// </summary>
        /// <returns>a collection of organisation summaries</returns>
        Task<IEnumerable<OrganisationSummary>> FindAllAsync();

        /// <summary>
        /// Check if a assessment organisation exists
        /// HEAD /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>bool</returns>
        bool Exists(string organisationId);

        /// <summary>
        /// Check if a assessment organisation exists
        /// HEAD /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(string organisationId);

        /// <summary>
        /// Get a collection of standards
        /// GET /assessment-organisations/{organisationId}/standards
        /// </summary>
        /// /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a collection of standards</returns>
        IEnumerable<StandardOrganisationSummary> FindAllStandardsByOrganisationId(string organisationId);

        /// <summary>
        /// Get a collection of standards
        /// GET /assessment-organisations/{organisationId}/standards
        /// </summary>
        /// /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a collection of standards</returns>
        Task<IEnumerable<StandardOrganisationSummary>> FindAllStandardsByOrganisationIdAsync(string organisationId);
    }
}