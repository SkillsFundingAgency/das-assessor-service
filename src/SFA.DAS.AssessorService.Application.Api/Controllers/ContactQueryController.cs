namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AssessorService.Application.Api.Middleware;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using SFA.DAS.AssessorService.Application.Api.Orchesrators;

    [Authorize]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {
        private readonly GetContactsOrchestrator _getContactsOrchestrator;

        public ContactQueryController(GetContactsOrchestrator getContactsOrchestrator)
        {
            _getContactsOrchestrator = getContactsOrchestrator;
        }

        [HttpGet("{endPointAssessorOrganisationId}", Name = "SearchContactsForAnOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            var contacts = await _getContactsOrchestrator.SearchContactsForAnOrganisation(endPointAssessorOrganisationId);
            return Ok(contacts);
        }

        [HttpGet("user/{userName}", Name = "SearchContactByUserName")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            var contacts = await _getContactsOrchestrator.SearchContactByUserName(userName);
            return Ok(contacts);
        }        
    }
}