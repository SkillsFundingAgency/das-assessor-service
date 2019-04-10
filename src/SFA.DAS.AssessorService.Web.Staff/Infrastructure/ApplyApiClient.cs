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
using SFA.DAS.AssessorService.Web.Staff.Services;
using Feedback = SFA.DAS.AssessorService.ApplyTypes.Feedback;
using Page = SFA.DAS.AssessorService.ApplyTypes.Page;

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

        public ApplyApiClient(string baseUri, ILogger<ApplyApiClient> logger, ITokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
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

        public async Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceId)
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/OpenApplications?sequenceId={sequenceId}");
        }

        public async Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/FeedbackAddedApplications");
        }

        public async Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/ClosedApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/OpenApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/FeedbackAddedApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/ClosedApplications");
        }

        public async Task ImportWorkflow(IFormFile file)
        {
            var formDataContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream())
                {Headers = {ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)}};
            formDataContent.Add(fileContent, file.Name, file.FileName);

            await _client.PostAsync($"/Import/Workflow", formDataContent);
        }

        public async Task<ApplyTypes.Application> GetApplication(Guid applicationId)
        {
            return await Get<ApplyTypes.Application>($"/Application/{applicationId}");
        }

        public async Task<ApplicationSequence> GetActiveSequence(Guid applicationId)
        {
            return await Get<ApplicationSequence>($"/Review/Applications/{applicationId}");
        }

        public async Task<ApplicationSequence> GetSequence(Guid applicationId, int sequenceId)
        {
            return await Get<ApplicationSequence>($"Application/{applicationId}/User/null/Sequences/{sequenceId}");
        }

        public async Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId)
        {
            return await Get<ApplicationSection>($"Application/{applicationId}/User/null/Sequences/{sequenceId}/Sections/{sectionId}");
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceId, int sectionId, bool isSectionComplete)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Evaluate",
                new { isSectionComplete });
        }

        public async Task<Page> GetPage(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            var page = await Get<Page>($"Application/{applicationId}/User/null/Sequence/{sequenceId}/Sections/{sectionId}/Pages/{pageId}");
            if(page != null) page.ApplicationId = applicationId;
            return page;
        }

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/AddFeedback",
                feedback);
        }

        public async Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/DeleteFeedback",
                feedbackId);
        }

        public async Task ReturnApplication(Guid applicationId, int sequenceId, string returnType)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceId}/Return", new { returnType });
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int pageId, string questionId, Guid userId, int sequenceId, int sectionId, string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            return await _client.GetAsync(new Uri($"/Download/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}", UriKind.Relative));
        }
        
        public async Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId,string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

           
            var downloadResponse = await _client.GetAsync(
                $"/Download/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}");
            return downloadResponse;
        }

        public async Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId,string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            
            var downloadResponse = await (await _client.GetAsync(
                $"/FileInfo/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}")).Content.ReadAsAsync<FileInfoResponse>();
            return downloadResponse;
        }


        public async Task UpdateFinancialGrade(Guid applicationId, FinancialApplicationGrade vmGrade)
        {
            await Post($"/Financial/{applicationId}/UpdateGrade", vmGrade);
        }

        public async Task StartFinancialReview(Guid applicationId)
        {
            await Post($"/Financial/{applicationId}/StartReview",new {applicationId}); 
        }

        public async Task<Organisation> GetOrganisationForApplication(Guid applicationId)
        {
            return await Get<Organisation>($"/Application/{applicationId}/Organisation");
        }

        public async Task StartApplicationReview(Guid applicationId, int sequenceId)
        {
            await Post($"/Review/Applications/{applicationId}/Sequences/{sequenceId}/StartReview", new { sequenceId });
        }

        public async Task<GetAnswersResponse> GetAnswer(Guid applicationId, string questionTag)
        {
            return await Get<GetAnswersResponse>($"/Answer/{questionTag}/{applicationId}");
        }

        public async Task<Contact> GetContact(Guid contactId)
        {
            return await Get<Contact>($"/Account/Contact/{contactId}");
        }

        public async Task UpdateRoEpaoApprovedFlag(Guid applicationId,Guid contactId, string endPointAssessorOrganisationId, bool roEpaoApprovedFlag)
        {
            await Post($"/organisations/{applicationId}/{contactId}/{endPointAssessorOrganisationId}/RoEpaoApproved/{roEpaoApprovedFlag}", new { roEpaoApprovedFlag });
        }
    }
}