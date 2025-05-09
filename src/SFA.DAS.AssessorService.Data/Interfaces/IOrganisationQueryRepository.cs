﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Data.Interfaces
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
        Task<Organisation> GetOrganisationByContactId(Guid contactId);
        Task<bool> CheckIfOrganisationHasContactsWithGovUkIdentifier(string endPointAssessorOrganisationId, Guid contactId);
        Task<bool> IsOfsOrganisation(int ukprn);

        Task<IEnumerable<Organisation>> GetOrganisationsByStandard(int standard);

        Task<PaginatedList<MergeLogEntry>> GetOrganisationMergeLogs(int pageSize, int pageIndex, string orderBy, string orderDirection, string primaryEPAOId, string secondaryEPAOId, string status);
        Task<MergeLogEntry> GetOrganisationMergeLogById(int id);
    }
}