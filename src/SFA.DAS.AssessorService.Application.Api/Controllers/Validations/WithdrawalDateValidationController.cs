using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Validations
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    public class WithdrawalDateValidationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WithdrawalDateValidationController> _logger;

        public WithdrawalDateValidationController(IMediator mediator, ILogger<WithdrawalDateValidationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet("Validations/WithdrawalDate/{id}")]
        public async Task<ActionResult<ApiValidationResult>> ValidateWithdrawalDate(Guid id, string q)
        {
            try
            {
                var application = await _mediator.Send(new GetApplicationRequest(id));
                var earliestDateOfWithdrawal =  await _mediator.Send(new GetEarliestWithdrawalDateRequest(application.OrganisationId, application.ApplyData.Apply.StandardCode));

                var dateComponents = q.Split(",");
                var enteredDate = new DateTime(int.Parse(dateComponents[2]), int.Parse(dateComponents[1]), int.Parse(dateComponents[0]));
                if (enteredDate < earliestDateOfWithdrawal.Date)
                    return new ApiValidationResult 
                    { 
                        IsValid = false,
                        ErrorMessages = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("", $"Date must not be before {earliestDateOfWithdrawal.Date:d MMM yyyy}")
                        }
                    };
                else
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