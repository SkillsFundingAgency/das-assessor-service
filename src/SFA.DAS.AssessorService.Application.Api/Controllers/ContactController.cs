using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
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
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;

        public ContactController(ILogger<ContactController> logger, IMediator mediator, IContactRepository contactRepository, IContactQueryRepository contactQueryRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
        }

        [HttpPost(Name = "CreateContact")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(ContactBoolResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateContact(
            [FromBody] CreateContactRequest createContactRequest)
        {
            _logger.LogInformation("Received Create Contact Request");

            var contactResponse =await _mediator.Send(createContactRequest);

            return CreatedAtRoute("CreateContact",
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

        [HttpPut("updateContactWithOrgAndStatus", Name = "UpdateContactWithOrgAndStatus")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContactWithOrgAndStatus([FromBody] UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest)
        {
            _logger.LogInformation("Received Update Contact Status Request");

            await _mediator.Send(updateContactWithOrgAndStausRequest);

            return NoContent();
        }

        [PerformValidation]
        [HttpPost("callback", Name= "Callback")]
        public async Task<ActionResult> Callback([FromBody] DfeSignInCallback callback)
        {
            _logger.LogInformation($"Received callback from DfE: Sub: {callback.Sub} SourceId: {callback.SourceId}");
            await _mediator.Send(new UpdateSignInIdRequest(Guid.Parse(callback.Sub), Guid.Parse(callback.SourceId)));
            return NoContent(); 
        }

        [HttpPost("createNewContactWithGivenId", Name = "CreateNewContactWithGivenId")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(Contact))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<Contact>> CreateNewContactWithGivenId([FromBody] Contact contact)
        {
            _logger.LogInformation($"Creating a new contact only with given Id");
            var newContact = await _contactRepository.CreateNewContact(contact);
            return Ok(newContact);
        }

        [HttpPost("associateDefaultRolesAndPrivileges", Name = "AssociateDefaultRolesAndPrivileges")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> AssociateDefaultRolesAndPrivileges([FromBody] Contact contact)
        {
            _logger.LogInformation($"Associating roles and privileges to a contact");
            await _contactRepository.AssociateRoleWithContact("SuperUser", contact);
            var privileges = await _contactQueryRepository.GetAllPrivileges();
            await _contactRepository.AssociatePrivilegesWithContact(contact.Id, privileges);
            return NoContent();
        }
        

    }
}