namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Domain;

    public interface IOrganisationRepository
    {     
        Task<Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation);
        Task<Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel);
        Task Delete(string endPointAssessorOrganisationId);
    }
}