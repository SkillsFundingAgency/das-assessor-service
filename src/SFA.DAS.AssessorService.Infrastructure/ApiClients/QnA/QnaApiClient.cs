﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA
{
    public class QnaApiClient : ApiClientBase, IQnaApiClient
    {
        public QnaApiClient(IQnaApiClientFactory clientFactory, ILogger<QnaApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/start"))
            {
                return await PostPutRequestWithResponseAsync<StartApplicationRequest, StartApplicationResponse>(request, startAppRequest);
            }
        }

        public async Task<T> GetApplicationData<T>(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData"))
            {
                return await RequestAndDeserialiseAsync<T>(request,
                    $"Could not find the application");
            }
        }

        public async Task<Dictionary<string, object>> GetApplicationDataDictionary(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData"))
            {
                return await RequestAndDeserialiseAsync<Dictionary<string, object>>(request,
                    $"Could not find the application");
            }
        }

        public async Task<CreateSnapshotResponse> SnapshotApplication(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/snapshot"))
            {
                return await RequestAndDeserialiseAsync<CreateSnapshotResponse>(request, $"Could not snapshot the requested application");
            }
        }

        public async Task<T> UpdateApplicationData<T>(Guid applicationId, T applicationData)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/applicationData"))
            {
                return await PostPutRequestWithResponseAsync<T, T>(request, applicationData);
            }
        }

        public async Task<Dictionary<string, object>> UpdateApplicationDataDictionary(Guid applicationId, Dictionary<string, object> applicationData)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/applicationData"))
            {
                return await PostPutRequestWithResponseAsync<Dictionary<string, object>, Dictionary<string, object>>(request, applicationData);
            }
        }

        public async Task<Sequence> GetApplicationActiveSequence(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/current"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }

        public async Task<string> GetQuestionTag(Guid applicationId, string questionTag)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData/{questionTag}"))
            {
                return await RequestAndDeserialiseAsync<string>(request,
                    $"Could not find the question tag");
            }
        }

        public async Task<Sequence> GetSequence(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceId}"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }

        public async Task<Sequence> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }

        public async Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceId}/sections"))
            {
                return await RequestAndDeserialiseAsync<List<Section>>(request,
                    $"Could not find the sections");
            }
        }

        public async Task<Section> GetSection(Guid applicationId, Guid sectionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}"))
            {
                return await RequestAndDeserialiseAsync<Section>(request,
                    $"Could not find the section");
            }
        }

        public async Task<Section> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}"))
            {
                return await RequestAndDeserialiseAsync<Section>(request,
                    $"Could not find the section");
            }
        }

        public async Task<Page> GetPage(Guid applicationId, Guid sectionId, string pageId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(request,
                    $"Could not find the page");
            }
        }

        public async Task<Page> GetPageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(request,
                    $"Could not find the page");
            }
        }

        public async Task<SkipPageResponse> SkipPage(Guid applicationId, Guid sectionId, string pageId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/skip"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new SetResultConverter());
                return await PostPutRequestWithResponseAsync<SkipPageResponse>(request, settings);
            }
        }

        public async Task<AddPageAnswerResponse> AddAnswersToMultipleAnswerPage(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/multiple"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new AddResultConverter());
                return await PostPutRequestWithResponseAsync<List<Answer>, AddPageAnswerResponse>(request, answer, settings);
            }
        }

        public async Task<Page> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/multiple/{answerId}"))
            {
                return await DeleteAsync<Page>(request);
            }
        }

        public async Task<SetPageAnswersResponse> SetPageAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new SetResultConverter());
                return await PostPutRequestWithResponseAsync<List<Answer>, SetPageAnswersResponse>(request, answer, settings);
            }
        }

        public async Task<ResetPageAnswersResponse> ResetSectionAnswers(Guid applicationId, int sequenceId, int sectionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sequences/{sequenceId}/sections/{sectionId}/reset"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new SetResultConverter());
                return await PostPutRequestWithResponseAsync<ResetPageAnswersResponse>(request, settings);
            }
        }

        public async Task<SetPageAnswersResponse> Upload(Guid applicationId, Guid sectionId, string pageId, IFormFileCollection files)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/upload"))
            {
                var formDataContent = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream())
                    { Headers = { ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType) } };
                    formDataContent.Add(fileContent, file.Name, file.FileName);
                }
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new SetResultConverter());
                return await PostPutRequestWithResponseAsync<SetPageAnswersResponse>(request, formDataContent, settings);
            }
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}"))
            {
                return await RequestToDownloadFileAsync(request,
                    $"Could not download file {fileName}");
            }
        }

        public async Task DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}"))
            {
                await DeleteAsync(request);
            }
        }

        public async Task<List<Sequence>> GetAllApplicationSequences(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences"))
            {
                return await RequestAndDeserialiseAsync<List<Sequence>>(request,
                    $"Could not find all sequences");
            }
        }

        public async Task<List<Section>> GetAllApplicationSections(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections"))
            {
                return await RequestAndDeserialiseAsync<List<Section>>(request,
                    $"Could not find all sections");
            }
        }

        public async Task<Page> UpdateFeedback(Guid applicationId, Guid sectionId, string pageId, Feedback feedback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/feedback"))
            {
                return await PostPutRequestWithResponseAsync<Feedback, Page>(request, feedback);
            }
        }

        public async Task<Page> DeleteFeedback(Guid applicationId, Guid sectionId, string pageId, Guid feedbackId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/feedback/{feedbackId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(request);
            }
        }

        public async Task<bool> AllFeedbackCompleted(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sequence/{sequenceId}/feedback/completed"))
            {
                return await RequestAndDeserialiseAsync<bool>(request,
                     $"Could not find the sections");
            }
        }

        private class SetResultConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(SetPageAnswersResponse);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                string nextAction = (string)jo["nextAction"];
                string nextActionId = (string)jo["nextActionId"];
                List<KeyValuePair<string, string>> errors = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(jo["validationErrors"]?.ToString(Formatting.None));
                SetPageAnswersResponse result;
                if (errors == null)
                    result = new SetPageAnswersResponse(nextAction, nextActionId);
                else
                    result = new SetPageAnswersResponse(errors);
                return result;
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private class AddResultConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AddPageAnswerResponse);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                Page page = JsonConvert.DeserializeObject<Page>(jo["page"].ToString(Formatting.None));
                List<KeyValuePair<string, string>> errors = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(jo["validationErrors"]?.ToString(Formatting.None));
                AddPageAnswerResponse result;
                if (errors == null)
                    result = new AddPageAnswerResponse(page);
                else
                    result = new AddPageAnswerResponse(errors);
                return result;
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
