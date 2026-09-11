using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IOrganisationStandardRepository
    {
        Task<IEnumerable<OrganisationStandard>> GetOrganisationStandardsByOrganisationIdAndStandardReference(string organisationId, string standardReference);
        Task<int> CreateOrganisationStandard(OrganisationStandard organisationStandard);
        Task<OrganisationStandardVersion> CreateOrganisationStandardVersion(OrganisationStandardVersion version);
        Task<OrganisationStandardVersion> GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(int organisationStandardId, string version);
        Task<OrganisationStandardVersion> UpdateOrganisationStandardVersion(OrganisationStandardVersion organisationStandardVersion);
        Task WithdrawOrganisation(string endPointAssessorOrganisationId, DateTime withdrawalDate);
        Task WithdrawStandard(string endPointAssessorOrganisationId, int standardCode, DateTime withdrawalDate);
    }
}
