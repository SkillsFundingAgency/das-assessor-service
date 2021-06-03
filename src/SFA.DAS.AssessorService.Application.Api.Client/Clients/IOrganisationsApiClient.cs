using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateOrganisationRequest = SFA.DAS.AssessorService.Api.Types.Models.CreateOrganisationRequest;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using OrganisationType = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationType;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<OrganisationResponse>> GetAll();
        Task<OrganisationResponse> Get(string ukprn);
        Task<Organisation> Get(Guid organisationId);
        Task<DateTime> GetEarliestWithdrawalDate(Guid organisationId, int? standardId);


        Task<OrganisationResponse> Create(CreateOrganisationRequest createOrganisationRequest);
        Task Update(UpdateOrganisationRequest updateOrganisationRequest);
        Task Delete(Guid id);

        Task<ValidationResponse> ValidateCreateOrganisation(string name, long? ukprn, int? organisationTypeId, string companyNumber, string charityNumber);
        Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, long? ukprn, int? organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice, string companyNumber, string charityNumber);

        Task<ValidationResponse> ValidateCreateContact(string firstName, string lastName, string organisationId, string email, string phone);
        Task<ValidationResponse> ValidateUpdateContact(string contactId, string firstName, string lastName, string email, string phoneNumber);

        Task<ValidationResponse> ValidateSearchStandards(string searchstring);

        Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas);
        Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus);

        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);

        Task UpdateEpaOrganisation(UpdateEpaOrganisationRequest updateEpaOrganisationRequest);

        Task<bool> AssociateOrganisationWithEpaContact(AssociateEpaOrganisationWithEpaContactRequest associateEpaOrganisationWithEpaContactRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationPrimaryContact(UpdateEpaOrganisationPrimaryContactRequest updateEpaOrganisationPrimaryContactRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationPhoneNumber(UpdateEpaOrganisationPhoneNumberRequest updateEpaOrganisationPhoneNumberRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationAddress(UpdateEpaOrganisationAddressRequest updateEpaOrganisationAddressRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationEmail(UpdateEpaOrganisationEmailRequest updateEpaOrganisationEmailRequest);
        Task<List<ContactResponse>> UpdateEpaOrganisationWebsiteLink(UpdateEpaOrganisationWebsiteLinkRequest updateEpaOrganisationWebsiteLinkRequest);

        Task<List<OrganisationType>> GetOrganisationTypes();
        
        Task SendEmailsToOrganisationUserManagementUsers(NotifyUserManagementUsersRequest notifyUserManagementUsersRequest);
        Task<OrganisationResponse> GetOrganisationByName(string name);
        Task<OrganisationResponse> GetOrganisationByUserId(Guid userId);
        Task<List<OrganisationStandardSummary>> GetOrganisationStandardsByOrganisation(string endPointAssessorOrganisationId);
        Task<OrganisationStandard> GetOrganisationStandardByOrganisationAndReference(string endPointAssessorOrganisationId, string standardReference);
        Task<PaginatedList<OrganisationSearchResult>> SearchForOrganisations(string searchTerm, int pageSize, int pageIndex);

        Task<bool> IsCompanyActivelyTrading(string companyNumber);
        Task<Company> GetCompanyDetails(string companyNumber);
        Task<Charity> GetCharityDetails(int charityNumber);

        Task<EpaOrganisationResponse> CreateEpaOrganisation(CreateEpaOrganisationRequest epaoOrganisationModel);
        Task<EpaOrganisation> GetEpaOrganisationById(string Id);
    }
}