using System.Threading.Tasks;

using SFA.DAS.AssessorService.Domain.DomainModels;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationRepository
    {     
        Task<Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);
        Task Delete(string endPointAssessorOrganisationId);
    }
}