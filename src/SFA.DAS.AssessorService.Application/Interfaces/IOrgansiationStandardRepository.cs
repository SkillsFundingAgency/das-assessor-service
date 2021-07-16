using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrgansiationStandardRepository
    {
        Task<OrganisationStandard> GetOrganisationStandardByOrganisationIdAndStandardReference(string organisationId, string standardReference);
        Task<OrganisationStandardVersion> CreateOrganisationStandardVersion(OrganisationStandardVersion version);
    }
}
