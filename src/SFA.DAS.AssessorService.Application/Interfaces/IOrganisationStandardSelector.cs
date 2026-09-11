using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public enum OrganisationStandardSelectionResult
    {
        Found,
        NotFound,
        RequiresNewRecord
    }

    public record OrganisationStandardSelectionOutcome
    {
        public OrganisationStandardSelectionResult Result { get; init; }
        public OrganisationStandard OrganisationStandard { get; init; }
        public OrganisationStandard TemplateOrganisationStandard { get; init; }
        public int TotalRowCount { get; init; }
        public int MatchingLarsCodeCount { get; init; }
    }

    public interface IOrganisationStandardSelector
    {
        OrganisationStandardSelectionOutcome Select(IEnumerable<OrganisationStandard> organisationStandards, int larsCode);
    }
}
