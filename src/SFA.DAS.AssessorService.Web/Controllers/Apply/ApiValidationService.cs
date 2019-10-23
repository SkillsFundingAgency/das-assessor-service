using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApiValidationService : IApiValidationService
    {
        private readonly IWebConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly string _clientApiCallValidationName;

        public ApiValidationService(IWebConfiguration config, ITokenService tokenService)
        {
            _config = config;
            _tokenService = tokenService;
            _clientApiCallValidationName = "ClientApiCall";
        }

        public async Task<ApiValidationResult> CallApiValidation(Page page, List<Answer> answers)
        {
            if (page.Questions.Any(q => q.Input.Type == _clientApiCallValidationName))
            {
                var validationResult = new ApiValidationResult();
                
                var apiBaseUri = new Uri(_config.ClientApiAuthentication.ApiBaseAddress, UriKind.Absolute);
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = apiBaseUri;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());    
                    
                    foreach (var questionWithClientApiCall in page.Questions.Where(q => q.Input.Validations.Any(v => v.Name == _clientApiCallValidationName)))
                    {
                        foreach (var apiCallValidation in questionWithClientApiCall.Input.Validations.Where(v => v.Name == _clientApiCallValidationName))
                        {
                            var result = await (await httpClient.GetAsync($"{apiCallValidation.Value}?q={answers.Single(a => a.QuestionId == questionWithClientApiCall.QuestionId)}"))
                                .Content.ReadAsAsync<ApiValidationResult>();

                            if (!result.IsValid)
                            {
                                validationResult.IsValid = false;
                                result.ErrorMessages.ForEach(m =>
                                {
                                    validationResult.ErrorMessages.Add(new KeyValuePair<string, string>(questionWithClientApiCall.QuestionId, m.Value));
                                });
                            }
                        }
                    }
                }

                return validationResult;
            }

            return new ApiValidationResult() {IsValid = true};
        }
    }
    
    
}