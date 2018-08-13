using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterRepository
    {
        Task<EpaOrganisation> CreateEpaOrganisation(EpaOrganisation organisation);
        Task<EpaOrganisation> GetEpaOrganisationById(Guid id);
        Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId);
        Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId);
        Task<bool> EpaOrganisationExistsWithUkprn(long ukprn);

        Task<bool> OrganisationTypeExists(int organisationTypeId);
    }
}
