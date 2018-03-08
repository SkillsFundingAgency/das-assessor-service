using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Attributes;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/contacts")]
    [ValidateBadRequest]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ContactOrchestrator _contactOrchestrator;

        public ContactController(
            ContactOrchestrator contactOrchestrator,
            ILogger<ContactController> logger
        )
        {
            _contactOrchestrator = contactOrchestrator;
            _logger = logger;
        }

        [HttpPost(Name = "CreateContract")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateContact(
            [FromBody] CreateContactRequest contactCreateViewModel)
        {
            _logger.LogInformation("Received Create Contact Request");

            var contactQueryViewModel = await _contactOrchestrator.CreateContact(contactCreateViewModel);

            return CreatedAtRoute("CreateContract",
                new { contactQueryViewModel.Username },
                contactQueryViewModel);
        }

        [HttpPut(Name = "UpdateContact")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest contactUpdateViewModel)
        {
            _logger.LogInformation("Received Update Contact Request");

            await _contactOrchestrator.UpdateContact(contactUpdateViewModel);

            return NoContent();
        }

        [HttpDelete(Name = "Delete")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(string userName)
        {
            _logger.LogInformation("Received Delete Contact Request");

            await _contactOrchestrator.DeleteContact(userName);

            return NoContent();
        }
    }
}