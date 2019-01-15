using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationQueryRepository
    {
        Task<IEnumerable<Organisation>> GetAllOrganisations();
        Task<Organisation> GetByUkPrn(long ukprn);
        Task<Organisation> Get(string endPointAssessorOrganisationId);
        Task<Organisation> Get(Guid id);

        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid organisationId);
        Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId);

        Task<int> GetEpaOrganisationStandardsCount(string endPointAssessorOrganisationId);
        Task<int> GetEpaoPipelineCount(string endPointAssessorOrganisationId);
    }
}