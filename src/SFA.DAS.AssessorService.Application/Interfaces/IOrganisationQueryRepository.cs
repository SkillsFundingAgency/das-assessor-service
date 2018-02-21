namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public interface IOrganisationQueryRepository
    {
        Task<IEnumerable<OrganisationQueryViewModel>> GetAllOrganisations();
        Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn);
        Task<OrganisationUpdateDomainModel> Get(Guid organisationId);

        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid organisationId);
        Task<bool> CheckIfOrganisationHasContacts(Guid organisationId);
    }
}