using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationQueryRepository
    {
        Task<IEnumerable<OrganisationResponse>> GetAllOrganisations();
        Task<OrganisationResponse> GetByUkPrn(int ukprn);
        Task<OrganisationDomainModel> Get(string endPointAssessorOrganisationId);

        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid organisationId);
        Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId);
    }
}