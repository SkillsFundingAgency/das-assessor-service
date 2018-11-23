using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApplyApiClient : IApplyApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApplyApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApplyApiClient(HttpClient client, ILogger<ApplyApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        private async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) ;
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<List<ApplyTypes.Application>> ReviewApplications()
        {
            return await Get<List<ApplyTypes.Application>>("/Review/Applications");
        }
        
        public async Task ImportWorkflow(IFormFile file)
        {
            var formDataContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream())
                {Headers = {ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)}};
            formDataContent.Add(fileContent, file.Name, file.FileName);

            await _client.PostAsync($"/Import/Workflow", formDataContent);
        }

        public async Task<ApplicationSequence> GetActiveSequence(Guid applicationId)
        {
            return await Get<ApplicationSequence>($"/Review/Applications/{applicationId}");
        }
        
        public async Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId)
        {
            return await Get<ApplicationSection>($"Application/{applicationId}/User/null/Sequences/{sequenceId}/Sections/{sectionId}");
        }

        public async Task<Page> GetPage(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            return await Get<Page>($"Application/{applicationId}/User/null/Sequence/{sequenceId}/Sections/{sectionId}/Pages/{pageId}");
        }

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, string message)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/AddFeedback",
                new {message, from = "Staff member", date = DateTime.UtcNow});
        }
    }
}