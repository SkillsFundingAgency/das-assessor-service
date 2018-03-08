using System.Threading.Tasks;

using SFA.DAS.AssessorService.Domain.DomainModels;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationRepository
    {     
        Task<OrganisationResponse> CreateNewOrganisation(CreateOrganisationDomainModel createOrganisationDomainModel);
        Task<OrganisationResponse> UpdateOrganisation(UpdateOrganisationDomainModel updateOrganisationDomainModel);
        Task Delete(string endPointAssessorOrganisationId);
    }
}