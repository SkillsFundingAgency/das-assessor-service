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
        private readonly ILogger<StandardService> _logger;
        private readonly IStandardRepository _standardRepository;

        public StandardService(CacheService cacheService, ILogger<StandardService> logger, IStandardRepository standardRepository)
        {
            _cacheService = cacheService;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task UpsertStandards(IEnumerable<GetStandardsListItem> standards)
        {
            Func<GetStandardsListItem, Standard> MapGetStandardsListItemToStandard = source => new Standard 
            {
                StandardUId = source.StandardUId,
                IfateReferenceNumber = source.IfateReferenceNumber,
                LarsCode = source.LarsCode,
                Title = source.Title,
                Version = source.Version,
                Level = source.Level,
                Status = source.Status,
                TypicalDuration = source.TypicalDuration,
                MaxFunding = source.MaxFunding,
                IsActive = source.IsActive,
                LastDateStarts = source.StandardDates?.LastDateStarts,
                EffectiveFrom = source.StandardDates?.EffectiveFrom,
                EffectiveTo = source.StandardDates?.EffectiveTo
            };

            await _standardRepository.DeleteAll();

            var tasks = standards.Select(MapGetStandardsListItemToStandard).Select(_standardRepository.Insert);

            await Task.WhenAll(tasks);
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

        public async Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId)
        {
            var results = await _standardRepository.GetEpaoRegisteredStandards(endPointAssessorOrganisationId, int.MaxValue, 1);
            return results.PageOfResults;
        }
    }
}
