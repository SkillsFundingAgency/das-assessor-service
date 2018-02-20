namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public interface IOrganisationRepository
    {
        Task<IEnumerable<OrganisationQueryViewModel>> GetAllOrganisations();
        Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn);
        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid id);

        Task<OrganisationQueryViewModel> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<OrganisationQueryViewModel> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);

        Task Delete(Guid id);
    }
}