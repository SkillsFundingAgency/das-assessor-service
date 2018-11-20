using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApplyApiClient : ApiClientBase, IApplyApiClient
    {
        public ApplyApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri,
            tokenService, logger)
        {
        }

        public ApplyApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(
            httpClient, tokenService, logger)
        {
        }

        public async Task ImportWorkflow(IFormFile file)
        {
            var formDataContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream())
                {Headers = {ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)}};
            formDataContent.Add(fileContent, file.Name, file.FileName);

            await HttpClient.PostAsync($"/Import/Workflow", formDataContent);
        }
    }
}