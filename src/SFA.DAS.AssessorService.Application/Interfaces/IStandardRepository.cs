﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardRepository
    {        
        Task<string> UpsertApprovedStandards(List<StandardCollation> standards);
        Task<string> UpsertNonApprovedStandards(List<StandardNonApprovedCollation> standards);

        Task<List<StandardCollation>> GetStandardCollations();
        Task<StandardCollation> GetStandardCollationByStandardId(int standardId);
        Task<StandardCollation> GetStandardCollationByReferenceNumber(string referenceNumber);
        
        Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollations();
        Task<StandardNonApprovedCollation> GetStandardNonApprovedCollationByReferenceNumber(string referenceNumber);

        Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId);
        Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId,int pageSize, int pageIndex);
        Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId,
        string orderBy, string orderDirection, int pageSize, int? pageIndex);
        Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId);

        // New Standard Version Methods
        Task<IEnumerable<Standard>> GetAllStandards();
        Task<IEnumerable<Standard>> GetStandardVersionsByLarsCode(int larsCode);
        Task<Standard> GetStandardVersionByStandardUId(string standardUId);
        
        /// <summary>
        /// Returns Specific version based on given lars code and version if the latter is supplied
        /// Or returns the latest version of a standard if only lars code supplied
        /// </summary>
        /// <param name="larsCode">if just lars code, latest version returned</param>
        /// <param name="version">optional parameter for specific version</param>
        /// <returns></returns>
        Task<Standard> GetStandardVersionByLarsCode(int larsCode, string version = null);
        /// <summary>
        /// Returns Specific version based on given ifate reference number and version if the latter is supplied
        /// Or returns the latest version of a standard if only ifate reference number is supplied
        /// </summary>
        /// <param name="iFateReferenceNumber">if just iFateReferenceNumber, latest version returned</param>
        /// <param name="version">optional parameter for specific version</param>
        /// <returns></returns>
        Task<Standard> GetStandardVersionByIFateReferenceNumber(string iFateReferenceNumber, string version = null);
        Task InsertStandards(IEnumerable<Standard> standards);
        Task InsertOptions(IEnumerable<StandardOption> optionsToInsert);
        Task<IEnumerable<StandardOptions>> GetAllStandardOptions();
        Task<IEnumerable<StandardOptions>> GetStandardOptionsForLatestStandardVersions();
        Task<StandardOptions> GetStandardOptionsByStandardUId(string standardUId);
        Task<StandardOptions> GetStandardOptionsByLarsCode(int larsCode);
        Task<StandardOptions> GetStandardOptionsByIFateReferenceNumber(string iFateReferenceNumber);
        Task Update(Standard standard);
        Task DeleteAllStandards();
        Task DeleteAllOptions();
        Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string endPointAssessorOrganisationId);
        Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string endPointAssessorOrganisationId, int larsCode);
    }

    public class EpoRegisteredStandardsResult
    {
        public IEnumerable<EPORegisteredStandards> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class EpaoPipelineStandardsResult
    {
        public IEnumerable<EpaoPipelineStandard> PageOfResults { get; set; }
        public int TotalCount { get; set; }
    }
}
