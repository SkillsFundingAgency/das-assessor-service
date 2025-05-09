﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;
using Swashbuckle.AspNetCore.Annotations;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {
        private readonly SearchOrganisationForContactsValidator _searchOrganisationForContactsValidator;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<ContactQueryController> _logger;
        private readonly IApiConfiguration _config;
        private readonly IMapper _mapper;

        public ContactQueryController(IContactQueryRepository contactQueryRepository,
            SearchOrganisationForContactsValidator searchOrganisationForContactsValidator,
            IMediator mediator,
            ILogger<ContactQueryController> logger, IApiConfiguration config, IMapper mapper)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _config = config;
            _searchOrganisationForContactsValidator = searchOrganisationForContactsValidator;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("privileges")]
        public async Task<IActionResult> GetAllPrivileges()
        {
            var privileges = await _contactQueryRepository.GetAllPrivileges();
            return Ok(privileges);
        }
        
        [HttpGet("user/{userId}/privileges")]
        public async Task<IActionResult> GetPrivilegesForContact(Guid userId)
        {
            var privileges = await _contactQueryRepository.GetPrivilegesFor(userId);
            return Ok(privileges);
        }

        [HttpGet("{endPointAssessorOrganisationId}", Name = "SearchContactsForAnOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            var contacts =
                _mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetContactsForEpao(endPointAssessorOrganisationId)).ToList();
            return Ok(contacts);
        }

        [HttpPost("getAll", Name = "GetAllContactsForOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForOrganisation([FromBody] GetAllContactsRequest request)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using epaoId = {request.EndPointAssessorOrganisationId}");

            ValidateEndPointAssessorOrganisation(request.EndPointAssessorOrganisationId);

            return Ok(await _mediator.Send(request));
        }

        [HttpPost("getAll/includePrivileges", Name = "GetAllContactsForOrganisationIncludePrivileges")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactIncludePrivilegesResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForOrganisationIncludePrivileges([FromBody] GetAllContactsIncludePrivilegesRequest request)
        {
            _logger.LogInformation(
                $"Received Search for Contacts and their Privileges using epaoId = {request.EndPointAssessorOrganisationId}");

            ValidateEndPointAssessorOrganisation(request.EndPointAssessorOrganisationId);

            return Ok(await _mediator.Send(request));
        }        

        [HttpGet("getAllWhoCanBePrimary/{endPointAssessorOrganisationId}", Name = "GetAllContactsWhoCanBePrimaryForOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsWhoCanBePrimaryForOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts who can be primary using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            return Ok(await _mediator.Send(new GetContactsWhoCanBePrimaryForOrganisationRequest(endPointAssessorOrganisationId)));
        }

        [HttpGet("username/{userName}", Name = "SearchContactByUserName")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            _logger.LogInformation($"Received Search Contact By UserName Request using user name = {userName}");

            var contact = await _contactQueryRepository.GetContact(userName);
            if (contact == null)
                throw new ResourceNotFoundException();
            return Ok(_mapper.Map<ContactResponse>(contact));
        }

        
        [HttpGet("email/{email}", Name = "SearchContactByEmail")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByEmail(string email)
        {
            var contact = await _contactQueryRepository.GetContactFromEmailAddress(email);
            if (contact == null)
                throw new ResourceNotFoundException();
            return Ok(_mapper.Map<ContactResponse>(contact));
        }

        [HttpGet("govidentifier/{govIdentifier}", Name = "SearchContactByGovIdentifier")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByGovIdentifier(string govIdentifier)
        {
            var contact = await _contactQueryRepository.GetContactByGovIdentifier(govIdentifier);
            if (contact == null)
                return NotFound();
            return Ok(_mapper.Map<ContactResponse>(contact));
        }
        
        [HttpGet("user/{id}", Name = "GetContactById")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactById(string id)
        {
            Contact contact = null;
            _logger.LogInformation($" Get Request using user id = {id}");
            try
            {
                var guidId = Guid.Parse(id);
                contact = await _contactQueryRepository.GetContactById(guidId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Failed to retrieve contact with id : {id}");
            }

            if (contact == null) { 
                throw new ResourceNotFoundException();
            }
         
            return Ok(_mapper.Map<ContactResponse>(contact));
        }

        [HttpGet("user/{id}/haveprivileges", Name = "DoesContactHavePrivileges")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactBoolResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DoesContactHavePrivileges(string id)
        {
            bool havePrivileges = false;

            _logger.LogInformation($" Get Request using user id = {id}");
            try
            {
                var guidId = Guid.Parse(id);

                var privileges = await _contactQueryRepository.GetPrivilegesFor(guidId);
                havePrivileges = privileges != null && privileges.Any();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to retrieve contact with id : {id}");
            }

            return Ok(new ContactBoolResponse(havePrivileges));
        }
        
        
        private void ValidateEndPointAssessorOrganisation(string endPointAssessorOrganisationId)
        {
            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);
        }
      
    }


    public class MigrateUserResult
    {
        public Guid NewUserId { get; set; }
    }

}