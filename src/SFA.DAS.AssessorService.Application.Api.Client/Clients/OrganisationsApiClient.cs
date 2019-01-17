using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    using AssessorService.Api.Types.Models;

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

        public async Task<ValidationResponse> ValidateCreateOrganisation(string name, string ukprn, string organisationTypeId)
        {

            var newName = SanitizeUrlParam(name);
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/validate-new?name={newName}&ukprn={ukprn}&organisationTypeId={organisationTypeId}")
            )
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    $"Could not check the validation for organisation [{name}]");
            }
        }

        public async Task<ValidationResponse> ValidateUpdateContact(string contactId, string displayName, string email,string phoneNumber)
        {
            var newName = SanitizeUrlParam(displayName);
            var newEmail = SanitizeUrlParam(email);
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/contacts/validate-existing?displayName={newName}&email={newEmail}&contactId={contactId}&phoneNumber={phoneNumber}"))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    $"Could not check the validation for contact [{newName}] against contactId [{contactId}]");
            }
        }
        
        public async Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, string ukprn, string organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice)
        {
            var newName = SanitizeUrlParam(name);
            var newAddress1 = SanitizeUrlParam(address1);
            var newAddress2 = SanitizeUrlParam(address2);
            var newAddress3 = SanitizeUrlParam(address3);
            var newAddress4 = SanitizeUrlParam(address4);
            var newPostcode = SanitizeUrlParam(postcode);

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/validate-existing?organisationId={organisationId}&name={newName}&ukprn={ukprn}" +
                                                                        $"&organisationTypeId={organisationTypeId}&address1={newAddress1}&address2={newAddress2}&address3={newAddress3}&address4={newAddress4}" +
                                                                        $"&postcode={newPostcode}&status={status}&actionChoice={actionChoice}"))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request, $"Could not check the validation for existing organisation [{organisationId}]");
            }
        }
        
        public async Task<ValidationResponse> ValidateCreateContact(string name, string organisationId,
            string email, string phone)
        {

            var newName = SanitizeUrlParam(name);
            var newEmail = SanitizeUrlParam(email);
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/contacts/validate-new?name={newName}&organisationId={organisationId}&email={newEmail}&phone={phone}"))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    $"Could not check the validation for contact [{name}] against organisation [{organisationId}]");
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

            var deliveryAreasString = deliveryAreas.Aggregate(string.Empty, (current, deliveryArea) => current + $"&DeliveryAreas={deliveryArea}");
            var contactIdString = contactId == null ? string.Empty : $"&contactId={contactId.Value}";

            var queryString =
                $"/api/ao/assessment-organisations/standards/validate-new?OrganisationId={organisationId}&StandardCode={standardId}&EffectiveFrom={effectiveFrom?.ToString("yyyy-MM-dd")}" +
                $"&EffectiveTo={effectiveTo?.ToString("yyyy-MM-dd")}{contactIdString}{deliveryAreasString}";
            using (var request = new HttpRequestMessage(HttpMethod.Get, queryString))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    "Could not check the validation for adding organisation standard using given details");
            }
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom,
            DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus)
        {
            var deliveryAreasString = deliveryAreas.Aggregate(string.Empty, (current, deliveryArea) => current + $"&DeliveryAreas={deliveryArea}");
            var contactIdString = contactId == null ? string.Empty : $"&contactId={contactId.Value}";
           
            var queryString =
                $"/api/ao/assessment-organisations/standards/validate-existing?OrganisationId={organisationId}&StandardCode={standardId}&EffectiveFrom={effectiveFrom?.ToString("yyyy-MM-dd")}" +
                $"&EffectiveTo={effectiveTo?.ToString("yyyy-MM-dd")}&ActionChoice={actionChoice}&organisationStandardStatus={organisationStandardStatus}&organisationStatus={organisationStatus}{contactIdString}{deliveryAreasString}";
            using (var request = new HttpRequestMessage(HttpMethod.Get, queryString))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    "Could not check the validation for adding organisation standard using given details");
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

        private string SanitizeUrlParam(string rawParam)
        {
            var result = rawParam;

            result = result?.Replace("%", "");
            result = result?.Replace("<", "");
            result = result?.Replace(">", "");
            result = result?.Replace("#", "");
            result = result?.Replace("{", "");
            result = result?.Replace("}", "");
            result = result?.Replace("|", "");
            result = result?.Replace("\\", "");
            result = result?.Replace("^", "");
            result = result?.Replace("~", "");
            result = result?.Replace("[", "");
            result = result?.Replace("]", "");
            result = result?.Replace("`", "%60");
            result = result?.Replace(";", "");
            result = result?.Replace("/", "");
            result = result?.Replace("?", "");
            result = result?.Replace("=", "");
            result = result?.Replace("&", "%26");
            result = result?.Replace("$", "");
            return result;
        }
    }

    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<OrganisationResponse>> GetAll();
        Task<OrganisationResponse> Get(string ukprn);
        Task<OrganisationResponse> Create(CreateOrganisationRequest organisationCreateViewModel);
        Task Update(UpdateOrganisationRequest organisationUpdateViewModel);
        Task Delete(Guid id);
        Task<ValidationResponse> ValidateCreateOrganisation(string name, string ukprn, string organisationTypeId);
        Task<ValidationResponse> ValidateUpdateContact(string contactId, string displayName, string email, string phoneNumber);
        Task<ValidationResponse> ValidateUpdateOrganisation(string organisationId, string name, string ukprn, string organisationTypeId, string address1, string address2, string address3, string address4, string postcode, string status, string actionChoice);
        Task<ValidationResponse> ValidateCreateContact(string name, string organisationId, string email, string phone);

        Task<ValidationResponse> ValidateSearchStandards(string searchstring);

        Task<ValidationResponse> ValidateCreateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas);
        Task<ValidationResponse> ValidateUpdateOrganisationStandard(string organisationId, int standardId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? contactId, List<int> deliveryAreas, string actionChoice, string organisationStandardStatus, string organisationStatus);
      
    }
}