using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {
        private readonly GetContactsOrchestrator _getContactsOrchestrator;
        private readonly ILogger<ContactQueryController> _logger;

        public ContactQueryController(GetContactsOrchestrator getContactsOrchestrator,
            ILogger<ContactQueryController> logger)
        {
            _logger = logger;
            _getContactsOrchestrator = getContactsOrchestrator;
        }

        [HttpGet("{endPointAssessorOrganisationId}", Name = "SearchContactsForAnOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var contacts =
                await _getContactsOrchestrator.SearchContactsForAnOrganisation(endPointAssessorOrganisationId);
            return Ok(contacts);
        }

        [HttpGet("user/{userName}", Name = "SearchContactByUserName")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            _logger.LogInformation($"Received Search Contact By UserName Request using user name = {userName}");

            var contacts = await _getContactsOrchestrator.SearchContactByUserName(userName);
            return Ok(contacts);
        }
    }
}