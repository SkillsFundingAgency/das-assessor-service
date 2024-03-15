using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApiValidationService : IApiValidationService
    {
        private readonly IAssessorApiClientFactory _clientFactory;
        private readonly ILogger<ApiValidationService> _logger;
        private readonly string _clientApiCallValidationName;

        public ApiValidationService(IAssessorApiClientFactory clientFactory, ILogger<ApiValidationService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _clientApiCallValidationName = "ClientApiCall";
        }

        public async Task<ApiValidationResult> CallApiValidation(Guid Id, Page page, List<Answer> answers)
        {
            if (page.Questions.Any(q => q.Input.Validations.Any(v => v.Name == _clientApiCallValidationName)))
            {
                _logger.LogInformation("Page has question with api validation");
                var validationResult = new ApiValidationResult() { IsValid = true };

                using (var httpClient = _clientFactory.CreateHttpClient())
                {
                    _logger.LogInformation("Auth set");
                    
                    foreach (var questionWithClientApiCall in page.Questions.Where(q => q.Input.Validations.Any(v => v.Name == _clientApiCallValidationName)))
                    {
                        _logger.LogInformation("Question with api call: " + questionWithClientApiCall.QuestionId);
                        foreach (var apiCallValidation in questionWithClientApiCall.Input.Validations.Where(v => v.Name == _clientApiCallValidationName))
                        {
                            _logger.LogInformation("Validation call: " + httpClient.BaseAddress.ToString() + apiCallValidation.Value);
                            
                            var result = await (await httpClient.GetAsync($"{apiCallValidation.Value}/{Id}?q={answers.Single(a => a.QuestionId == questionWithClientApiCall.QuestionId).Value}"))
                                .Content.ReadAsAsync<ApiValidationResult>();

                            _logger.LogInformation("Api call result: Valid:" + result.IsValid);
                            
                            if (!result.IsValid)
                            {
                                validationResult.IsValid = false;
                                result.ErrorMessages.ForEach(m =>
                                {
                                    
                                    _logger.LogInformation("Api call result: Error message:" + m.Value);

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