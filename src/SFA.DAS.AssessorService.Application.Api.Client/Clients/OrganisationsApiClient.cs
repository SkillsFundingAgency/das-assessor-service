using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    using AssessorService.Api.Types.Models;
    using SFA.DAS.AssessorService.Api.Types.Models.Register;
    using System.Net;

    public class OrganisationsApiClient : ApiClientBase, IOrganisationsApiClient
    {
        public OrganisationsApiClient(string baseUri, ITokenService tokenService,
            ILogger<OrganisationsApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public OrganisationsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
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

        public async Task<OrganisationResponse>  GetOrganisationByName(string name)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/{WebUtility.UrlEncode(name)}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationResponse>(request,
                    $"Could not find the organisations");
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

        public async Task<OrganisationResponse> Create(CreateOrganisationRequest organisationCreateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisations/"))
            {
                return await PostPutRequestWithResponse<CreateOrganisationRequest, OrganisationResponse>(request,
                    organisationCreateViewModel);
            }
        }

        public async Task<ValidationResponse> ValidateCreateOrganisation(string name, long? ukprn, int? organisationTypeId, string companyNumber, string charityNumber)
        {
            var validationRequest = new CreateEpaOrganisationValidationRequest
            {
                Name = name,
                Ukprn = ukprn,
                OrganisationTypeId = organisationTypeId,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/validate-new/"))
            {
                return await PostPutRequestWithResponse<CreateEpaOrganisationValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, long? ukprn, int? organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice, string companyNumber, string charityNumber)
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
                CharityNumber = charityNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/validate-existing/"))
            {
                return await PostPutRequestWithResponse<UpdateEpaOrganisationValidationRequest, ValidationResponse>(request,
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
                return await PostPutRequestWithResponse<CreateEpaContactValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateContact(string contactId, string displayName, string firstName, string lastName, string email,string phoneNumber)
        {
            var validationRequest = new UpdateEpaOrganisationContactValidationRequest
            {
                ContactId = contactId,
                DisplayName = displayName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/contacts/validate-existing/"))
            {
                return await PostPutRequestWithResponse<UpdateEpaOrganisationContactValidationRequest, ValidationResponse>(request,
                    validationRequest);
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

        public async Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom,
            DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas)
        {
            var validationRequest = new CreateEpaOrganisationStandardValidationRequest
            {
                OrganisationId = organisationId,
                StandardCode = standardId,
                EffectiveFrom = effectiveFrom?.Date,
                EffectiveTo = effectiveTo?.Date,
                ContactId = contactId.HasValue ? contactId.Value.ToString() : string.Empty,
                DeliveryAreas = deliveryAreas
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/standards/validate-new/"))
            {
                return await PostPutRequestWithResponse<CreateEpaOrganisationStandardValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom,
            DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus)
        {
            var validationRequest = new UpdateEpaOrganisationStandardValidationRequest
            {
                OrganisationId = organisationId,
                StandardCode = standardId,
                EffectiveFrom = effectiveFrom?.Date,
                EffectiveTo = effectiveTo?.Date,
                ContactId = contactId.HasValue ? contactId.Value.ToString() : string.Empty,
                DeliveryAreas = deliveryAreas,
                ActionChoice = actionChoice,
                OrganisationStandardStatus = organisationStandardStatus,
                OrganisationStatus = organisationStatus
            };
          
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ao/assessment-organisations/standards/validate-new/"))
            {
                return await PostPutRequestWithResponse<UpdateEpaOrganisationStandardValidationRequest, ValidationResponse>(request,
                    validationRequest);
            }
        }

        public async Task Update(UpdateOrganisationRequest organisationUpdateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/organisations/"))
            {
                await PostPutRequest(request, organisationUpdateViewModel);
            }
        }

        public async Task Delete(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/organisations/"))
            {
                await Delete(request);
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

        public async Task<List<OrganisationType>> GetOrganisationTypes()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/organisation-types"))
            {
                return await RequestAndDeserialiseAsync<List<OrganisationType>>(request,
                    $"Could not retrieve organisation types.");
            }
        }


      
        public async Task SendEmailsToOrgApprovedUsers(EmailAllApprovedContactsRequest emailAllApprovedContactsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put,
                $"/api/v1/organisations/NotifyAllApprovedUsers"))
            {
                 await PostPutRequest (request, emailAllApprovedContactsRequest);
            }
        }
        
    }

    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<OrganisationResponse>> GetAll();
        Task<OrganisationResponse> Get(string ukprn);
        Task<OrganisationResponse> Create(CreateOrganisationRequest organisationCreateViewModel);
        Task Update(UpdateOrganisationRequest organisationUpdateViewModel);
        Task Delete(Guid id);

        Task<ValidationResponse> ValidateCreateOrganisation(string name, long? ukprn, int? organisationTypeId, string companyNumber, string charityNumber);
        Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, long? ukprn, int? organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice, string companyNumber, string charityNumber);

        Task<ValidationResponse> ValidateCreateContact(string firstName, string lastName, string organisationId, string email, string phone);
        Task<ValidationResponse> ValidateUpdateContact(string contactId, string displayName, string firstName, string lastName, string email, string phoneNumber);

        Task<ValidationResponse> ValidateSearchStandards(string searchstring);

        Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas);
        Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus);
        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);
        Task<List<OrganisationType>> GetOrganisationTypes();
        Task SendEmailsToOrgApprovedUsers(EmailAllApprovedContactsRequest emailAllApprovedContactsRequest);
        Task<OrganisationResponse> GetOrganisationByName(string name);
    }
}