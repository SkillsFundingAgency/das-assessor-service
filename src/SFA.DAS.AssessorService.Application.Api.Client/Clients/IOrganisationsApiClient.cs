using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CreateOrganisationRequest = SFA.DAS.AssessorService.Api.Types.Models.CreateOrganisationRequest;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using OrganisationType = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationType;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<OrganisationResponse>> GetAll();
        Task<OrganisationResponse> Get(string ukprn);
        Task<Organisation> Get(Guid organisationId);
        Task<List<Contact>> GetOrganisationContacts(Guid organisationId);
        Task<DateTime> GetEarliestWithdrawalDate(Guid organisationId, int? standardId);


        Task<OrganisationResponse> Create(CreateOrganisationRequest createOrganisationRequest);
        Task Update(UpdateOrganisationRequest updateOrganisationRequest);
        Task Delete(Guid id);
        Task WithdrawOrganisation(WithdrawOrganisationRequest request);

        Task<ValidationResponse> ValidateCreateOrganisation(string name, long? ukprn, int? organisationTypeId, string companyNumber, string charityNumber, string recognitionNumber);
        Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, long? ukprn, int? organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice, string companyNumber, string charityNumber, string recognitionNumber);

        Task<ValidationResponse> ValidateCreateContact(string firstName, string lastName, string organisationId, string email, string phone);
        Task<ValidationResponse> ValidateUpdateContact(string contactId, string firstName, string lastName, string email, string phoneNumber);

        Task<ValidationResponse> ValidateSearchStandards(string searchstring);

        Task<EpaoStandardResponse> AddOrganisationStandard(OrganisationStandardAddRequest organisationStandardAddRequest);
        Task WithdrawStandard(WithdrawStandardRequest request);

        Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardCode, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas);
        Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int organisationStandardId, int standardCode, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus);
        Task<ValidationResponse> ValidateUpdateOrganisationStandardVersion(int organisationStandardId, string version, DateTime? effectiveFrom, DateTime? effectiveTo);

        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);

        Task UpdateEpaOrganisation(UpdateEpaOrganisationRequest updateEpaOrganisationRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationPrimaryContact(UpdateEpaOrganisationPrimaryContactRequest updateEpaOrganisationPrimaryContactRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationPhoneNumber(UpdateEpaOrganisationPhoneNumberRequest updateEpaOrganisationPhoneNumberRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationAddress(UpdateEpaOrganisationAddressRequest updateEpaOrganisationAddressRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationEmail(UpdateEpaOrganisationEmailRequest updateEpaOrganisationEmailRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationWebsiteLink(UpdateEpaOrganisationWebsiteLinkRequest updateEpaOrganisationWebsiteLinkRequest);

        Task<List<OrganisationType>> GetOrganisationTypes();

        Task SendEmailsToOrganisationUserManagementUsers(NotifyUserManagementUsersRequest notifyUserManagementUsersRequest);
        Task<OrganisationResponse> GetOrganisationByUserId(Guid userId);
        Task<List<OrganisationStandardSummary>> GetOrganisationStandardsByOrganisation(string endPointAssessorOrganisationId);
        Task<IEnumerable<AppliedStandardVersion>> GetAppliedStandardVersionsForEPAO(string endPointAssessorOrganisationId, string standardReference);
        Task<PaginatedList<OrganisationSearchResult>> SearchForOrganisations(string searchTerm, int pageSize, int pageIndex);

        Task<bool> IsCompanyActivelyTrading(string companyNumber);
        Task<Company> GetCompanyDetails(string companyNumber);
        Task<Charity> GetCharityDetails(int charityNumber);

        Task<EpaOrganisationResponse> CreateEpaOrganisation(CreateEpaOrganisationRequest epaoOrganisationModel);
        Task<EpaOrganisation> GetEpaOrganisationById(string Id);

        Task<OrganisationStandardVersion> OrganisationStandardVersionOptIn(string endPointAssessorOrganisationId,
           string standardReference, string version, DateTime? effectiveFrom, DateTime? effectiveTo, Guid contactId);

        Task<OrganisationStandardVersion> OrganisationStandardVersionOptOut(string endPointAssessorOrganisationId,
           string standardReference, string version, DateTime? effectiveFrom, DateTime? effectiveTo, Guid contactId);

        Task<string> UpdateEpaOrganisationStandardVersion(UpdateOrganisationStandardVersionRequest request);

        Task<bool> IsOfsOrganisation(EpaOrganisation organisation);
    }
}