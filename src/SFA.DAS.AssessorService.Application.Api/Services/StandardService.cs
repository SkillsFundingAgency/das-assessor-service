using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Mapping.Structs;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class StandardService : IStandardService
    {
        private readonly CacheService _cacheService;
        private readonly ILogger<StandardService> _logger;
        private readonly IStandardRepository _standardRepository;

        public StandardService(CacheService cacheService, ILogger<StandardService> logger, IStandardRepository standardRepository)
        {
            _cacheService = cacheService;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<IEnumerable<Standard>> GetAllStandardVersions()
        {
            var results = await _cacheService.RetrieveFromCache<IEnumerable<Standard>>("Standards");

            if (results != null)
                return results;

            var standards = await _standardRepository.GetAllStandards();

            await _cacheService.SaveToCache("Standards", standards, 8);
            return standards;
        }

        public async Task<IEnumerable<Standard>> GetLatestStandardVersions()
        {
            var results = await _cacheService.RetrieveFromCache<IEnumerable<Standard>>("LatestStandards");

            if (results != null)
                return results;

            var standards = await _standardRepository.GetLatestStandardVersions();

            await _cacheService.SaveToCache("LatestStandards", standards, 8);
            return standards;
        }

        public async Task<IEnumerable<Standard>> GetStandardVersionsByIFateReferenceNumber(string iFateReferenceNumber)
        {
            IEnumerable<Standard> standards = null;

            try
            {
                standards = await _standardRepository.GetStandardVersionsByIFateReferenceNumber(iFateReferenceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD VERSION: Failed to get for iFateReferenceNumber: {iFateReferenceNumber}");
            }

            return standards;
        }

        public async Task<Standard> GetStandardVersionById(string id, string version = null)
        {
            Standard standard = null;
            try
            {
                var standardId = new StandardId(id);

                switch (standardId.IdType)
                {
                    case StandardId.StandardIdType.LarsCode:
                        standard = await _standardRepository.GetStandardVersionByLarsCode(standardId.LarsCode, version);
                        break;
                    case StandardId.StandardIdType.IFateReferenceNumber:
                        standard = await _standardRepository.GetStandardVersionByIFateReferenceNumber(standardId.IFateReferenceNumber, version);
                        break;
                    case StandardId.StandardIdType.StandardUId:
                        standard = await _standardRepository.GetStandardVersionByStandardUId(standardId.StandardUId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("id", "StandardId was not of type StandardUId, LarsCode or IfateReferenceNumber");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD VERSION: Failed to get for standard id: {id}");
            }

            return standard;
        }

        public async Task<IEnumerable<Standard>> GetStandardVersionsByLarsCode(int larsCode)
        {
            IEnumerable<Standard> standards = null;

            try
            {
                standards = await _standardRepository.GetStandardVersionsByLarsCode(larsCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD: Failed to get for standard id: {larsCode}");
            }

            return standards;
        }

        public async Task<Standard> GetStandardVersionByStandardUId(string standardUId)
        {
            Standard standard = null;

            try
            {
                standard = await _standardRepository.GetStandardVersionByStandardUId(standardUId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD: Failed to get for standard id: {standardUId}");
            }

            return standard;
        }

        public async Task<IEnumerable<StandardOptions>> GetAllStandardOptions()
        {
            try
            {
                var options = await _standardRepository.GetAllStandardOptions();

                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "STANDARD OPTIONS: Failed to get standard options");
            }

            return null;
        }

        public async Task<IEnumerable<StandardOptions>> GetStandardOptionsForLatestStandardVersions()
        {
            try
            {
                var options = await _standardRepository.GetStandardOptionsForLatestStandardVersions();

                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "STANDARD OPTIONS: Failed to get options for latest version of each standard");
            }

            return null;
        }

        public async Task<StandardOptions> GetStandardOptionsByStandardId(string id)
        {
            StandardOptions options = null;
            try
            {
                var standardId = new StandardId(id);
                switch (standardId.IdType)
                {
                    case StandardId.StandardIdType.LarsCode:
                        options = await _standardRepository.GetStandardOptionsByLarsCode(standardId.LarsCode);
                        break;
                    case StandardId.StandardIdType.IFateReferenceNumber:
                        options = await _standardRepository.GetStandardOptionsByIFateReferenceNumber(standardId.IFateReferenceNumber);
                        break;
                    case StandardId.StandardIdType.StandardUId:
                        options = await _standardRepository.GetStandardOptionsByStandardUId(standardId.StandardUId);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD OPTIONS: Failed to get standard options for id {id}");
            }

            return options;
        }

        public async Task<bool> GetCoronationEmblemForStandardReferenceAndVersion(string iFateReferenceNumber, string version)
        {
            var result = await _standardRepository.GetCoronationEmblemForStandardReferenceAndVersion(iFateReferenceNumber, version);
            return result;
        }

        public async Task<StandardOptions> GetStandardOptionsByStandardIdAndVersion(string id, string version)
        {
            Standard standard = new Standard();

            try
            {
                var standardId = new StandardId(id);

                switch (standardId.IdType)
                {
                    case StandardId.StandardIdType.IFateReferenceNumber:
                        standard = await _standardRepository.GetStandardVersionByIFateReferenceNumber(standardId.IFateReferenceNumber, version);
                        break;
                    case StandardId.StandardIdType.LarsCode:
                        standard = await _standardRepository.GetStandardVersionByLarsCode(standardId.LarsCode, version);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not find standard with id: {id} and Version: {version}");

                return null;
            }

            return await GetStandardOptionsByStandardId(standard.StandardUId);
        }

        public async Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId)
        {
            var results = await _standardRepository.GetEpaoRegisteredStandards(endPointAssessorOrganisationId, int.MaxValue, 1);
            return results.PageOfResults;
        }

        public async Task<IEnumerable<OrganisationStandardVersion>> GetEPAORegisteredStandardVersions(string endPointAssessorOrganisationId, int? larsCode = null)
        {
            if (larsCode.HasValue && larsCode.Value > 0)
            {
                var versionsOfStandard = await _standardRepository.GetEpaoRegisteredStandardVersions(endPointAssessorOrganisationId, larsCode.Value);
                    
                return versionsOfStandard.Select(version => (OrganisationStandardVersion)version);
            }

            var versions = await _standardRepository.GetEpaoRegisteredStandardVersions(endPointAssessorOrganisationId);

            return versions.Select(version => (OrganisationStandardVersion)version);
        }

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(string endPointAssessorOrganisationId, string iFateReferenceNumber)
        {
            return await _standardRepository.GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(endPointAssessorOrganisationId, iFateReferenceNumber);
        }
    }
}
