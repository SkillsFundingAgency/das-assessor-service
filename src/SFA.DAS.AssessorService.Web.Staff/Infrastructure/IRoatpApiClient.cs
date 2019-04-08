﻿
namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IRoatpApiClient
    {
       Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
       Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
       Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummary();
       Task<IEnumerable<ProviderType>> GetProviderTypes();
       Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int providerTypeId);
       Task<bool> CreateOrganisation(CreateOrganisationRequest organisationRequest);
       Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn);
       Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber);
       Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber);
       Task<OrganisationSearchResults> Search(string searchTerm);
    }
}
