using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        public ContactQueryController(IContactQueryRepository contactQueryRepository,
            SearchOrganisationForContactsValidator searchOrganisationForContactsValidator,
            IMediator mediator,
            ILogger<ContactQueryController> logger)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _searchOrganisationForContactsValidator = searchOrganisationForContactsValidator;
            _mediator = mediator;
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
                Mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetContacts(endPointAssessorOrganisationId)).ToList();
            return Ok(contacts);
        }

        [HttpGet("get-all/{endPointAssessorOrganisationId}", Name = "GetAllContactsForOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            var contacts =
                Mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetAllContacts(endPointAssessorOrganisationId)).ToList();
            return Ok(contacts);
        }

        [HttpGet("user/{userName}", Name = "SearchContactByUserName")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            _logger.LogInformation($"Received Search Contact By UserName Request using user name = {userName}");

            var contact = await _contactQueryRepository.GetContact(userName);
            if (contact == null)
                throw new ResourceNotFoundException();
            return Ok(Mapper.Map<ContactResponse>(contact));
        }

        [HttpGet("{endPointAssessorOrganisationId}/withprivileges", Name = "GetAllContactsWithTheirPrivilages")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsWithTheirPrivileges(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts and their Privileges using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            return Ok(await _mediator.Send(new GetContactsRequest(endPointAssessorOrganisationId)));
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
         
            return Ok(Mapper.Map<ContactResponse>(contact));
        }
    }
}