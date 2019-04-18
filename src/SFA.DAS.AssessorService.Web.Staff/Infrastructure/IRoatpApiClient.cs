
using System;

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
       Task<IEnumerable<ProviderType>> GetProviderTypes();
       Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);
       Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId);
       Task<IEnumerable<RemovedReason>> GetRemovedReasons();
       Task<bool> CreateOrganisation(CreateOrganisationRequest organisationRequest);
       Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn);
       Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber);
       Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber);
       Task<OrganisationSearchResults> Search(string searchTerm);
       Task<bool> UpdateOrganisationLegalName(UpdateOrganisationLegalNameRequest request);
       Task<bool> UpdateOrganisationTradingName(UpdateOrganisationTradingNameRequest request);
       Task<bool> UpdateOrganisationStatus(UpdateOrganisationStatusRequest request);
        Task<bool> UpdateOrganisationType(UpdateOrganisationTypeRequest request);



        Task<bool> UpdateOrganisationParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeRequest request);
       Task<bool> UpdateOrganisationFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordRequest request);
       Task<bool> UpdateOrganisationProviderType(UpdateOrganisationProviderTypeRequest request);
       Task<bool> UpdateOrganisationUkprn(UpdateOrganisationUkprnRequest request);
    }
}
