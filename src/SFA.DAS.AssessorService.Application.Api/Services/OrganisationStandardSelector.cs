using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class OrganisationStandardSelector : IOrganisationStandardSelector
    {
        private readonly ILogger<OrganisationStandardSelector> _logger;

        public OrganisationStandardSelector(ILogger<OrganisationStandardSelector> logger)
        {
            _logger = logger;
        }

        public OrganisationStandardSelectionOutcome Select(IEnumerable<OrganisationStandard> organisationStandards, int larsCode)
        {
            var rows = organisationStandards.ToList();

            if (rows.Count == 0)
            {
                return new OrganisationStandardSelectionOutcome
                {
                    Result = OrganisationStandardSelectionResult.NotFound,
                    TotalRowCount = 0,
                    MatchingLarsCodeCount = 0
                };
            }

            if (rows.Count == 1)
            {
                return new OrganisationStandardSelectionOutcome
                {
                    Result = OrganisationStandardSelectionResult.Found,
                    OrganisationStandard = rows[0],
                    TotalRowCount = 1,
                    MatchingLarsCodeCount = 0
                };
            }

            var matches = rows.Where(r => r.StandardCode == larsCode).ToList();

            if (matches.Count == 1)
            {
                return new OrganisationStandardSelectionOutcome
                {
                    Result = OrganisationStandardSelectionResult.Found,
                    OrganisationStandard = matches[0],
                    TotalRowCount = rows.Count,
                    MatchingLarsCodeCount = matches.Count
                };
            }

            if (matches.Count > 1)
            {
                var activeCount = matches.Count(r => r.EffectiveTo == null);
                if (activeCount != 1)
                {
                    _logger.LogWarning(
                        "OrganisationStandard selection for EndPointAssessorOrganisationId {OrganisationId}, StandardReference {StandardReference}, LarsCode {LarsCode} matched {MatchingLarsCodeCount} records with no single active record - falling back to the most recent",
                        rows[0].EndPointAssessorOrganisationId, rows[0].StandardReference, larsCode, matches.Count);
                }

                var selected = matches
                    .OrderByDescending(r => r.EffectiveTo == null)
                    .ThenByDescending(r => r.EffectiveFrom)
                    .First();

                return new OrganisationStandardSelectionOutcome
                {
                    Result = OrganisationStandardSelectionResult.Found,
                    OrganisationStandard = selected,
                    TotalRowCount = rows.Count,
                    MatchingLarsCodeCount = matches.Count
                };
            }

            var template = rows.OrderByDescending(r => r.EffectiveFrom).First();

            return new OrganisationStandardSelectionOutcome
            {
                Result = OrganisationStandardSelectionResult.RequiresNewRecord,
                TemplateOrganisationStandard = template,
                TotalRowCount = rows.Count,
                MatchingLarsCodeCount = 0
            };
        }
    }
}
