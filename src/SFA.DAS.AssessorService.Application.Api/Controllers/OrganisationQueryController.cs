﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
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
using SFA.DAS.AssessorService.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/organisations")]
    public class OrganisationQueryController : Controller
    {
        private readonly ILogger<OrganisationQueryController> _logger;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly IStringLocalizer<OrganisationQueryController> _localizer;
        private readonly IWebConfiguration _config;

        public OrganisationQueryController(
            ILogger<OrganisationQueryController> logger, IOrganisationQueryRepository organisationQueryRepository, UkPrnValidator ukPrnValidator, IStringLocalizer<OrganisationQueryController> localizer,
            IWebConfiguration config
        )
        {
            _logger = logger;
            _organisationQueryRepository = organisationQueryRepository;
            _ukPrnValidator = ukPrnValidator;
            _localizer = localizer;
            _config = config;
        }
        
        [HttpGet("ukprn/{ukprn}", Name = "SearchOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
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

        [HttpGet("{*name}", Name = "GetOrganisationByName")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationByName(string name)
        {
            var decodedName = WebUtility.UrlDecode(name);
            _logger.LogInformation($"Received request to retrieve Organisation {decodedName}");
            
            var organisation = await _organisationQueryRepository.GetOrganisationByName(decodedName);
            if(organisation == null)
            {
                var ex = new ResourceNotFoundException(name);
                throw ex;
            }
            
            return Ok(Mapper.Map<OrganisationResponse>(organisation));
        }
    }
}
