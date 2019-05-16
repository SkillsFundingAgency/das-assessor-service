using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Clients;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/ukrlp/")]
    [ValidateBadRequest]
    public class UkrlpController :Controller
    { 
        private readonly ILogger<UkrlpController> _logger;
        private readonly IUkrlpApiClient _client;
        private readonly IUkrlpProcessingService _ukrlpProcessingService;

        public UkrlpController(ILogger<UkrlpController> logger,  IUkrlpApiClient client, IUkrlpProcessingService ukrlpProcessingService)
        {
            _logger = logger;
            _client = client;
            _ukrlpProcessingService = ukrlpProcessingService;
        }


        [HttpGet("provider-details/{ukprn}", Name = "GetProviderDetails")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UkrlpProviderDetails))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetProviderDetails(long ukprn)
        {
            _logger.LogInformation("Get Provider details");
            var results = await _client.GetTrainingProviderByUkprn(ukprn);
            var providerDetails = _ukrlpProcessingService.ProcessDetails(results);
            return Ok(providerDetails);
        }

        
    }
}
