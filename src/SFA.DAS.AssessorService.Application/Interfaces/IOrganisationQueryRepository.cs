using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOrganisationQueryRepository
    {
        Task<IEnumerable<Organisation>> GetAllOrganisations();
        Task<Organisation> GetByUkPrn(int ukprn);
        Task<Organisation> Get(string endPointAssessorOrganisationId);

        Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId);
        Task<bool> CheckIfAlreadyExists(Guid organisationId);
        Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId);
    }
}