﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
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
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/{ukprn}"))
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

        public async Task<ValidationResponse> ValidateCreateOrganisation(string name, string ukprn,
            string organisationTypeId)
        {

            var newName = SanitizeUrlParam(name);
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/ao/assessment-organisations/validate?name={newName}&ukprn={ukprn}&organisationTypeId={organisationTypeId}"))
            {
                return await RequestAndDeserialiseAsync<ValidationResponse>(request,
                    $"Could not check the validation for organisation [{name}]");
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
    }
}