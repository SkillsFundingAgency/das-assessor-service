using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/organisations")]
    public class OrganisationQueryController : Controller
    {
        private readonly ILogger<OrganisationQueryController> _logger;
        private readonly IMediator _mediator;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly IStringLocalizer<OrganisationQueryController> _localizer;

        public OrganisationQueryController(
            ILogger<OrganisationQueryController> logger, IMediator mediator, IOrganisationQueryRepository organisationQueryRepository, 
            UkPrnValidator ukPrnValidator, IStringLocalizer<OrganisationQueryController> localizer)
        {
            _logger = logger;
            _mediator = mediator;
            _organisationQueryRepository = organisationQueryRepository;
            _ukPrnValidator = ukPrnValidator;
            _localizer = localizer;
        }
        
        [HttpGet("ukprn/{ukprn}", Name = "SearchOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchOrganisation(int ukprn)
        {
            _logger.LogInformation($"Received Search for an Organisation Request using ukprn {ukprn}");

            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                throw new BadRequestException(result.Errors[0].ErrorMessage);

            var organisation = Mapper.Map<OrganisationResponse>(await _organisationQueryRepository.GetByUkPrn(ukprn));
            if (organisation == null)
            {
                var ex = new ResourceNotFoundException(
                    string.Format(_localizer[ResourceMessageName.NoAssesmentProviderFound].Value, ukprn));
                throw ex;
            }

            return Ok(organisation);
        }
        
        [HttpGet(Name="GetAllOrganisations")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<OrganisationResponse>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllOrganisations()
        {
            _logger.LogInformation("Received request to retrieve All Organisations");

            var organisations =
                Mapper.Map<List<OrganisationResponse>>(await _organisationQueryRepository.GetAllOrganisations());
                
            return Ok(organisations);
        }

        [HttpGet("organisation/{id}", Name = "GetOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Organisation))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisation(Guid id)
        {
            _logger.LogInformation($"Received request to retrieve Organisation {id}");

            var organisation =
                await _organisationQueryRepository.Get(id);

            return Ok(organisation);
        }

        [HttpGet("organisation/{id}/contacts", Name = "GetOrganisationContacts")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationContacts(Guid id)
        {
            _logger.LogInformation($"Received request to retrieve contacts for Organisation: {id}");

            return Ok(await _mediator.Send(new GetContactsForOrganisationRequest(id)));
        }
        
        [HttpGet("forContact/{userId}")]
        public async Task<IActionResult> GetOrganisationForContact(Guid userId)
        {
            var organisation = await _organisationQueryRepository.GetOrganisationByContactId(userId);
            if(organisation == null)
            {
                throw new ResourceNotFoundException(userId.ToString());
            }
            
            return Ok(Mapper.Map<Organisation,OrganisationResponse>(organisation));
        }

        [HttpGet("organisation/earliest-withdrawal/{id}", Name = "GetOrganisationEarliestWithdrawal")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationEarliestWithdrawal(Guid id)
        { 
            _logger.LogInformation($"Received request to retrieve earliest withdrawal for Organisation: {id}");
            
            return Ok(await _mediator.Send(new GetEarliestWithdrawalDateRequest(id)));
        }

        [HttpGet("organisation/earliest-withdrawal/{id}/{standardId}", Name = "GetOrganisationStandardEarliestWithdrawal")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationStandardEarliestWithdrawal(Guid id, int standardId)
        {
            _logger.LogInformation($"Received request to retrieve earliest withdrawal for Standard: {standardId} of Organisation: {id}");
            
            return Ok(await _mediator.Send(new GetEarliestWithdrawalDateRequest(id, standardId)));
        }
    }
}
