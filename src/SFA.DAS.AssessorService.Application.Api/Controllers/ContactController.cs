using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/contacts")]
    [ValidateBadRequest]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IMediator _mediator;

        public ContactController(ILogger<ContactController> logger, IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(Name = "CreateContract")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateContact(
            [FromBody] CreateContactRequest createContactRequest)
        {
            _logger.LogInformation("Received Create Contact Request");

            var contactResponse = Mapper.Map<ContactResponse>(await _mediator.Send(createContactRequest));

            return CreatedAtRoute("CreateContract",
                new { Username = contactResponse.Username },
                contactResponse);
        }

        [HttpPut(Name = "UpdateContact")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest updateContactRequest)
        {
            _logger.LogInformation("Received Update Contact Request");

            await _mediator.Send(updateContactRequest);

            return NoContent();
        }

        [HttpPut("status", Name = "UpdateContactStatus")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContactStatus([FromBody] UpdateContactStatusRequest updateContactStatusRequest)
        {
            _logger.LogInformation("Received Update Contact Status Request");

            await _mediator.Send(updateContactStatusRequest);

            return NoContent();
        }

        [HttpDelete(Name = "Delete")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(string userName)
        {
            _logger.LogInformation("Received Delete Contact Request");

            try
            {
                var deleteContactRequest = new DeleteContactRequest()
                {
                    UserName = userName
                };

                await _mediator.Send(deleteContactRequest);
            }
            catch (NotFound)
            {
                throw new ResourceNotFoundException();
            }

            return NoContent();
        }
    }
}