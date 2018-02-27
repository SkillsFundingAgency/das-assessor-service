namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Domain;
    using SFA.DAS.AssessorService.Api.Types;

    public interface IOrganisationRepository
    {     
        Task<Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);
        Task Delete(Guid id);
    }
}