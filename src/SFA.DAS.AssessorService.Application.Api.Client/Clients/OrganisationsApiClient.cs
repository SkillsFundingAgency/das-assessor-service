using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
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
using System.Net.Http;
using System.Threading.Tasks;

using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OrganisationsApiClient : ApiClientBase, IOrganisationsApiClient
    {
        public OrganisationsApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<OrganisationResponse>> GetAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<OrganisationResponse>>(request,
                    $"Could not find the organisations");
            }
        }

        public async Task<OrganisationResponse> GetOrganisationByUserId(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/forContact/{userId}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationResponse>(request,
                    $"Could not find the organisation for userId {userId}");
            }
        }

        public async Task<OrganisationResponse> Get(string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/ukprn/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationResponse>(request,
                    $"Could not find the organisation {ukprn}");
            }
        }

        public async Task<Organisation> Get(Guid organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/organisation/{organisationId}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(request, $"Could not find the organisation {organisationId}");
            }
        }

        public async Task<List<Contact>> GetOrganisationContacts(Guid organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/organisations/organisation/{organisationId}/contacts"))
            {
                return await RequestAndDeserialiseAsync<List<Contact>>(request, $"Could not find the organisation {organisationId} contacts");
            }
        }

        public async Task<DateTime> GetEarliestWithdrawalDate(Guid organisationId, int? standardCode)
        {
            if (standardCode.HasValue)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/organisation/earliest-withdrawal/{organisationId}/{standardCode}"))
                {
                    return await RequestAndDeserialiseAsync<DateTime>(request, $"Could not get the earliest withdrawal for the standard {standardCode} of the organisation {organisationId}");
                }
            }
            else
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/organisation/earliest-withdrawal/{organisationId}"))
                {
                    return await RequestAndDeserialiseAsync<DateTime>(request, $"Could not get the earliest withdrawal for the organisation {organisationId}");
                }
            }
        }

        public async Task<OrganisationResponse> Create(CreateOrganisationRequest createOrganisationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisations/"))
            {
                return await PostPutRequestWithResponseAsync<CreateOrganisationRequest, OrganisationResponse>(request,
                    createOrganisationRequest);
            }
        }

        public async Task Update(UpdateOrganisationRequest updateOrganisationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/organisations/"))
            {
                await PostPutRequestAsync(request, updateOrganisationRequest);
            }
        }

        public async Task Delete(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/organisations/"))
            {
                await DeleteAsync(request);
            }
        }

        public async Task WithdrawOrganisation(WithdrawOrganisationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisations/withdraw"))
            {
                await PostPutRequestAsync(httpRequest, request);
            }
        }

        public async Task<EpaOrganisationResponse> CreateEpaOrganisation(CreateEpaOrganisationRequest epaoOrganisationModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaOrganisationRequest, EpaOrganisationResponse>(request,
                    epaoOrganisationModel);
            }
        }

        public async Task<ValidationResponse> ValidateCreateOrganisation(string name, long? ukprn, int? organisationTypeId, string companyNumber, string charityNumber, string recognitionNumber)
        {
            var validationRequest = new CreateEpaOrganisationValidationRequest
            {
                Name = name,
                Ukprn = ukprn,
                OrganisationTypeId = organisationTypeId,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber,
                RecognitionNumber = recognitionNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/validate-new/"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaOrganisationValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, long? ukprn, int? organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice, string companyNumber, string charityNumber, string recognitionNumber)
        {
            var validationRequest = new UpdateEpaOrganisationValidationRequest
            {
                OrganisationId = organisationId,
                Name = name,
                Ukprn = ukprn,
                OrganisationTypeId = organisationTypeId,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Address4 = address4,
                Postcode = postcode,
                Status = status,
                ActionChoice = actionChoice,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber,
                RecognitionNumber = recognitionNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/validate-existing/"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateCreateContact(string firstName, string lastName, string organisationId, string email, string phone)
        {
            var validationRequest = new CreateEpaContactValidationRequest
            {
                OrganisationId = organisationId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/contacts/validate-new/"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaContactValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateContact(string contactId, string firstName, string lastName, string email, string phoneNumber)
        {
            var validationRequest = new UpdateEpaOrganisationContactValidationRequest
            {
                ContactId = contactId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ao/assessment-organisations/contacts/validate-existing/"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationContactValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<List<ContactResponse>> UpdateEpaOrganisationPrimaryContact(UpdateEpaOrganisationPrimaryContactRequest updateEpaOrganisationPrimaryContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/ao/assessment-organisations/update-primary-contact"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationPrimaryContactRequest, List<ContactResponse>>(request,
                    updateEpaOrganisationPrimaryContactRequest);
            }
        }

        public async Task<List<ContactResponse>> UpdateEpaOrganisationPhoneNumber(UpdateEpaOrganisationPhoneNumberRequest updateEpaOrganisationPhoneNumberRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/ao/assessment-organisations/update-phone-number"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationPhoneNumberRequest, List<ContactResponse>>(request,
                    updateEpaOrganisationPhoneNumberRequest);
            }
        }

        public async Task<List<ContactResponse>> UpdateEpaOrganisationAddress(UpdateEpaOrganisationAddressRequest updateEpaOrganisationAddressRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/ao/assessment-organisations/update-address"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationAddressRequest, List<ContactResponse>>(request,
                    updateEpaOrganisationAddressRequest);
            }
        }

        public async Task<List<ContactResponse>> UpdateEpaOrganisationEmail(UpdateEpaOrganisationEmailRequest updateEpaOrganisationEmailRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/ao/assessment-organisations/update-email"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationEmailRequest, List<ContactResponse>>(request,
                    updateEpaOrganisationEmailRequest);
            }
        }

        public async Task<List<ContactResponse>> UpdateEpaOrganisationWebsiteLink(UpdateEpaOrganisationWebsiteLinkRequest updateEpaOrganisationWebsiteLinkRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/ao/assessment-organisations/update-website-link"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationWebsiteLinkRequest, List<ContactResponse>>(request,
                    updateEpaOrganisationWebsiteLinkRequest);
            }
        }

        public async Task<ValidationResponse> ValidateSearchStandards(string searchstring)
        {

            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/standards/validate/search/{searchstring}"))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    $"Could not check the validation for standard using [{searchstring.Trim()}]");
            }
        }

        public async Task<EpaoStandardResponse> AddOrganisationStandard(OrganisationStandardAddRequest organisationStandardAddRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/organisationstandard"))
            {
                return await PostPutRequestWithResponseAsync<OrganisationStandardAddRequest, EpaoStandardResponse>(request,
                    organisationStandardAddRequest);
            }
        }

        public async Task WithdrawStandard(WithdrawStandardRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/organisationstandard/withdraw"))
            {
                await PostPutRequestAsync(httpRequest, request);
            }
        }

        public async Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardCode, DateTime? effectiveFrom,
            DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas)
        {
            var validationRequest = new CreateEpaOrganisationStandardValidationRequest
            {
                OrganisationId = organisationId,
                StandardCode = standardCode,
                EffectiveFrom = effectiveFrom?.Date,
                EffectiveTo = effectiveTo?.Date,
                ContactId = contactId.HasValue ? contactId.Value.ToString() : string.Empty,
                DeliveryAreas = deliveryAreas
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/standards/validate-new/"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaOrganisationStandardValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int organisationStandardId, int standardCode, DateTime? effectiveFrom,
            DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus)
        {
            var validationRequest = new UpdateEpaOrganisationStandardValidationRequest
            {
                OrganisationId = organisationId,
                OrganisationStandardId = organisationStandardId,
                StandardCode = standardCode,
                EffectiveFrom = effectiveFrom?.Date,
                EffectiveTo = effectiveTo?.Date,
                ContactId = contactId.HasValue ? contactId.Value.ToString() : string.Empty,
                DeliveryAreas = deliveryAreas,
                ActionChoice = actionChoice,
                OrganisationStandardStatus = organisationStandardStatus,
                OrganisationStatus = organisationStatus
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/standards/validate-existing/"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationStandardValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisationStandardVersion(int organisationStandardId, string version, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var validationRequest = new UpdateEpaOrganisationStandardVersionValidationRequest
            {
                OrganisationStandardId = organisationStandardId,
                OrganisationStandardVersion = version,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/standards/version/validate-existing/"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationStandardVersionValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisation(string endPointAssessorOrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/{endPointAssessorOrganisationId}"))
            {
                return await RequestAndDeserialiseAsync<EpaOrganisation>(request,
                    $"Could not retrieve details for the organisation with an Id of {endPointAssessorOrganisationId}");
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisationById(string Id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/{Id}"))
            {
                return await RequestAndDeserialiseAsync<EpaOrganisation>(request,
                    $"Could not retrieve details for the organisation with an Id of {Id}");
            }
        }

        public async Task UpdateEpaOrganisation(UpdateEpaOrganisationRequest updateEpaOrganisationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ao/assessment-organisations/"))
            {
                await PostPutRequestAsync(request, updateEpaOrganisationRequest);
            }
        }

        public async Task<List<AssessorService.Api.Types.Models.AO.OrganisationType>> GetOrganisationTypes()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/organisation-types"))
            {
                return await RequestAndDeserialiseAsync<List<AssessorService.Api.Types.Models.AO.OrganisationType>>(request,
                    $"Could not retrieve organisation types.");
            }
        }

        public async Task SendEmailsToOrganisationUserManagementUsers(NotifyUserManagementUsersRequest notifyUserManagementUsersRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put,
                $"/api/v1/organisations/NotifyUserManagementUsers"))
            {
                await PostPutRequestAsync(request, notifyUserManagementUsersRequest);
            }
        }

        public async Task<List<OrganisationStandardSummary>> GetOrganisationStandardsByOrganisation(string endPointAssessorOrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
               $"/api/ao/assessment-organisations/{endPointAssessorOrganisationId}/standards"))
            {
                return await RequestAndDeserialiseAsync<List<OrganisationStandardSummary>>(request,
                    $"Could not retrieve standards for organisation with Id of {endPointAssessorOrganisationId}");
            }
        }

        public async Task<IEnumerable<AppliedStandardVersion>> GetAppliedStandardVersionsForEPAO(string endPointAssessorOrganisationId, string standardReference)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
               $"/api/ao/assessment-organisations/{endPointAssessorOrganisationId}/standardversions/{standardReference}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<AppliedStandardVersion>>(request,
                    $"Could not retrieve standard versions for organisation with Id of {endPointAssessorOrganisationId} and standard reference {standardReference}", true);
            }
        }

        public async Task<PaginatedList<OrganisationSearchResult>> SearchForOrganisations(string searchTerm, int pageSize, int pageIndex)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/v1/organisationsearch/organisations?searchTerm={searchTerm}&pageSize={pageSize}&pageIndex={pageIndex}"))
                {
                    return await RequestAndDeserialiseAsync<PaginatedList<OrganisationSearchResult>>(request,
                        $"Could not retrieve organisations for search {searchTerm}.");
                }
            }
            catch (HttpRequestException err)
            {
                if (err.Message.Contains("204"))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> IsCompanyActivelyTrading(string companyNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisationsearch/company/{companyNumber}/isActivelyTrading"))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not retrieve trading details for the organisation with a company number of {companyNumber}");
            }
        }

        public async Task<Company> GetCompanyDetails(string companyNumber)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisationsearch/company/{companyNumber}"))
                {
                    return await RequestAndDeserialiseAsync<Company>(request, $"Could not retrieve details for the organisation with a company number of {companyNumber}");
                }
            }
            catch (HttpRequestException err)
            {
                if (err.Message.Contains("204"))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Charity> GetCharityDetails(int charityNumber)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisationsearch/charity/{charityNumber}"))
                {
                    return await RequestAndDeserialiseAsync<Charity>(request, $"Could not retrieve details for the organisation with a charity number of {charityNumber}");
                }
            }
            catch (HttpRequestException err)
            {
                if (err.Message.Contains("204"))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<OrganisationStandardVersion> OrganisationStandardVersionOptIn(string endPointAssessorOrganisationId,
            string standardReference, string version, DateTime? effectiveFrom, DateTime? effectiveTo, Guid contactId)
        {
            var optInRequest = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                StandardReference = standardReference,
                Version = version,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                ContactId = contactId,
                OptInRequestedAt = DateTime.Now
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisationstandardversion/opt-in"))
            {
                return await PostPutRequestWithResponseAsync<OrganisationStandardVersionOptInRequest, OrganisationStandardVersion>(request, optInRequest);
            }
        }

        public async Task<OrganisationStandardVersion> OrganisationStandardVersionOptOut(string endPointAssessorOrganisationId,
            string standardReference, string version, DateTime? effectiveFrom, DateTime? effectiveTo, Guid contactId)
        {
            var optOutRequest = new OrganisationStandardVersionOptOutRequest
            {
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                StandardReference = standardReference,
                Version = version,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                ContactId = contactId,
                OptOutRequestedAt = DateTime.Now
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisationstandardversion/opt-out"))
            {
                return await PostPutRequestWithResponseAsync<OrganisationStandardVersionOptOutRequest, OrganisationStandardVersion>(request, optOutRequest);
            }
        }

        public async Task<string> UpdateEpaOrganisationStandardVersion(UpdateOrganisationStandardVersionRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/organisationstandardversion/update"))
            {
                var result = await PostPutRequestWithResponseAsync<UpdateOrganisationStandardVersionRequest, EpaoStandardVersionResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<bool> IsOfsOrganisation(EpaOrganisation organisation)
        {
            long ukprn = organisation.Ukprn.Value;
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/is-ofs/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not determine whether organisation with UKPRN {ukprn} is an OfS organisation.");
            }
        }
    }
}