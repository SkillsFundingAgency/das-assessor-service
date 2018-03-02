namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Domain;

    public interface IOrganisationQueryRepository
    {
        Task<IEnumerable<Organisation>> GetAllOrganisations();
        Task<Organisation> GetByUkPrn(int ukprn);
        Task<OrganisationQueryDomainModel> Get(string endPointAssessorOrganisationId);

        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid organisationId);
        Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId);
    }
}