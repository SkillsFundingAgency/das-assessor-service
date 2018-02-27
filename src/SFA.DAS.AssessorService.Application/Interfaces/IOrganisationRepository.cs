namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.Api.Types;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public interface IOrganisationRepository
    {     
        Task<Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);
        Task Delete(Guid id);
    }
}