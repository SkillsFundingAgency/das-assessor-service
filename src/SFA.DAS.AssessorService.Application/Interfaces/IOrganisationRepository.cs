namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public interface IOrganisationRepository
    {       
        Task<IEnumerable<Organisation>> GetAllOrganisations();
        Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn);
        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<OrganisationQueryViewModel> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task DeleteOrganisationByEpaoId(string epaoId);
    }
}