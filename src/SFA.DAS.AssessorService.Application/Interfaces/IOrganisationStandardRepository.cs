using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationStandardRepository
    {
        Task<OrganisationStandard> GetOrganisationStandardByOrganisationIdAndStandardReference(string organisationId, string standardReference);
        Task<OrganisationStandardVersion> CreateOrganisationStandardVersion(OrganisationStandardVersion version);
        Task<OrganisationStandardVersion> GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(int organisationStandardId, string version);
        Task<OrganisationStandardVersion> UpdateOrganisationStandardVersion(OrganisationStandardVersion organisationStandardVersion);
        Task WithdrawOrganisation(string endPointAssessorOrganisationId, DateTime withdrawalDate);
        Task WithdrawStandard(string endPointAssessorOrganisationId, int standardCode, DateTime withdrawalDate);
    }
}
