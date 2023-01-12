﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly IWebConfiguration _config;

        public ContactQueryController(IContactQueryRepository contactQueryRepository,
            SearchOrganisationForContactsValidator searchOrganisationForContactsValidator,
            IMediator mediator,
            ILogger<ContactQueryController> logger, IWebConfiguration config)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _config = config;
            _searchOrganisationForContactsValidator = searchOrganisationForContactsValidator;
            _mediator = mediator;
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            var contacts =
                Mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetContactsForEpao(endPointAssessorOrganisationId)).ToList();
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            _logger.LogInformation($"Received Search Contact By UserName Request using user name = {userName}");

            var contact = await _contactQueryRepository.GetContact(userName);
            if (contact == null)
                throw new ResourceNotFoundException();
            return Ok(Mapper.Map<ContactResponse>(contact));
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
                _logger.LogError(e, $"Failed to retrieve contact with id : {id}");
            }

            if (contact == null)
            {
                throw new ResourceNotFoundException();
            }

            return Ok(Mapper.Map<ContactResponse>(contact));
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

        [HttpGet("signInId/{signInId}", Name = "GetContactBySignInId")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactBySignInId(string signInId)
        {
            Contact contact = null;
            _logger.LogInformation($" Get Request using signin id = {signInId}");
            try
            {
                var guidId = Guid.Parse(signInId);
                contact = await _contactQueryRepository.GetBySignInId(guidId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to retrieve contact with signin id : {signInId}");
            }

            if (contact == null)
            {
                throw new ResourceNotFoundException();
            }

            return Ok(Mapper.Map<ContactResponse>(contact));
        }

        [HttpPost("MigrateUsers", Name = "MigrateUsers")]
        public async Task<ActionResult> MigrateUsers()
        {
            var endpoint = new Uri(new Uri(_config.LoginService.MetadataAddress), "/Migrate");
            using (var httpClient = new HttpClient())
            {
                var usersToMigrate = await _contactQueryRepository.GetUsersToMigrate();
                foreach (var user in usersToMigrate)
                {
                    var result = await httpClient.PostAsJsonAsync(endpoint, new
                    {
                        ClientId = _config.LoginService.ClientId,
                        GivenName = user.GivenNames,
                        FamilyName = user.FamilyName,
                        Email = user.Email
                    });

                    var migrateResult = await result.Content.ReadAsAsync<MigrateUserResult>();

                    await _contactQueryRepository.UpdateMigratedContact(user.Id, migrateResult.NewUserId);
                }
            }


            return Ok();
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