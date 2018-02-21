namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public interface IOrganisationRepository
    {     
        Task<OrganisationQueryViewModel> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<OrganisationQueryViewModel> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);
        Task Delete(Guid id);
    }
}