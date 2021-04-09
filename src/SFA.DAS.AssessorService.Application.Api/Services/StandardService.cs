using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class StandardService : IStandardService
    {
        private readonly CacheService _cacheService;
        private readonly IOuterApiClient _outerApiClient;
        private readonly ILogger<StandardService> _logger;
        private readonly IStandardRepository _standardRepository;

        public StandardService(CacheService cacheService, IOuterApiClient outerApiClient, ILogger<StandardService> logger, IStandardRepository standardRepository)
        {
            _cacheService = cacheService;
            _outerApiClient = outerApiClient;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<IEnumerable<StandardCollation>> GetAllStandards()
        {
            var results = await _cacheService.RetrieveFromCache<IEnumerable<StandardCollation>>("StandardCollations");

            if (results != null)
                return results;

            var standardCollations = await _standardRepository.GetStandardCollations();

            await _cacheService.SaveToCache("StandardCollations", standardCollations, 8);
            return standardCollations;
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

        public async Task<StandardCollation> GetStandard(int standardId)
        {
            StandardCollation standardCollation = null;

            try
            {
                standardCollation = await _standardRepository.GetStandardCollationByStandardId(standardId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD COLLATION: Failed to get for standard id: {standardId}");
            }

            return standardCollation;
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

        public async Task<StandardCollation> GetStandard(string referenceNumber)
        {
            StandardCollation standardCollation = null;

            try
            {
                standardCollation = await _standardRepository.GetStandardCollationByReferenceNumber(referenceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD COLLATION: Failed to get for standard reference: {referenceNumber}");
            }

            return standardCollation;
        }


        public async Task<IEnumerable<StandardOptions>> GetStandardOptions()
        {
            try
            {
                var standardOptionsResponse = await _outerApiClient.Get<GetStandardOptionsListResponse>(new GetStandardOptionsRequest());

                return standardOptionsResponse.StandardOptions.Select(standard => new StandardOptions
                {
                    StandardUId = standard.StandardUId,
                    StandardCode = standard.LarsCode,
                    StandardReference = standard.IfateReferenceNumber,
                    Version = standard.Version.ToString("#.0"),
                    CourseOption = standard.Options
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "STANDARD OPTIONS: Failed to get standard options");
            }

            return null;
        }

        public async Task<StandardOptions> GetStandardOptionsByStandardId(string id)
        {
            try
            {
                var standard = await _outerApiClient.Get<StandardDetailResponse>(new GetStandardByIdRequest(id));

                return new StandardOptions
                {
                    StandardUId = standard.StandardUId,
                    StandardCode = standard.LarsCode,
                    StandardReference = standard.IfateReferenceNumber,
                    Version = standard.Version.ToString("#.0"),
                    CourseOption = standard.Options
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD OPTIONS: Failed to get standard options for id {id}");
            }

            return null;
        }

        public async Task<StandardOptions> GetStandardOptionsByStandardReferenceAndVersion(string standardReference, string version)
        {
            Standard standard;

            try
            {
                standard = await _standardRepository.GetStandardByStandardReferenceAndVersion(standardReference, version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not find standard with StandardReference: {standardReference} and Version: {version}");

                return null;
            }

            return await GetStandardOptionsByStandardId(standard.StandardUId);
        }

        public async Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId)
        {
            var results = await _standardRepository.GetEpaoRegisteredStandards(endPointAssessorOrganisationId, int.MaxValue, 1);
            return results.PageOfResults;
        }
    }
}
