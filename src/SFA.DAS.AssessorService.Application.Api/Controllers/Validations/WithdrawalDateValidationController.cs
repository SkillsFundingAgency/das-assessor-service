using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Validations
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    public class WithdrawalDateValidationController : Controller
    {
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IMediator _mediator;
        private readonly ILogger<WithdrawalDateValidationController> _logger;

        public WithdrawalDateValidationController(IQnaApiClient qnaApiClient, IMediator mediator, ILogger<WithdrawalDateValidationController> logger)
        {
            _qnaApiClient = qnaApiClient;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("Validations/WithdrawalDate/{id}")]
        public async Task<ActionResult<ApiValidationResult>> ValidateWithdrawalDate(Guid id, string q)
        {
            try
            {
                var application = await _mediator.Send(new GetApplicationRequest(id));
                var appData = await _qnaApiClient.GetApplicationData(application.ApplicationId);

                if ((appData.PipelinesCount ?? 0) != 0)
                {
                    var dateComponents = q.Split(",");
                    var enteredDate = new DateTime(int.Parse(dateComponents[2]), int.Parse(dateComponents[1]), int.Parse(dateComponents[0]));
                    if (enteredDate < appData.EarliestDateOfWithdrawal.Date)
                        return new ApiValidationResult
                        {
                            IsValid = false,
                            ErrorMessages = new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("", $"Date must not be before {appData.EarliestDateOfWithdrawal.Date:d MMM yyyy}")
                            }
                        };
                }

                return new ApiValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during withdrawal date validation for {q} : { ex.Message}");

                return new ApiValidationResult
                {
                    IsValid = false,
                    ErrorMessages = new List<KeyValuePair<string, string>>()
                };
            }
        }
    }
}