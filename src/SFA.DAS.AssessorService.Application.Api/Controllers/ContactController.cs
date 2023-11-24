using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(ContactBoolResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateContact(
            [FromBody] CreateContactRequest createContactRequest)
        {
            _logger.LogInformation("Received Create Contact Request");

            var contactResponse = await _mediator.Send(createContactRequest);

            return CreatedAtRoute("CreateContact",
                contactResponse);
        }

        [HttpPut(Name = "UpdateContact")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest updateContactRequest)
        {
            _logger.LogInformation("Received Update Contact Request");

            await _mediator.Send(updateContactRequest);

            return NoContent();
        }

        [HttpPut("status", Name = "UpdateContactStatus")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContactStatus([FromBody] UpdateContactStatusRequest updateContactStatusRequest)
        {
            _logger.LogInformation("Received Update Contact Status Request");

            await _mediator.Send(updateContactStatusRequest);

            return NoContent();
        }
        
        [HttpPut("govlogin", Name = "UpdateGovLogin")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<Contact>> UpdateGovLoginContactDetails([FromBody] UpdateContactGovLoginRequest updateContactStatusRequest)
        {
            _logger.LogInformation("Received Update Gov Login Contact Request");

            var result = await _mediator.Send(updateContactStatusRequest);

            return Ok(result.Contact);
        }

        [HttpDelete(Name = "Delete")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
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
            catch (NotFoundException)
            {
                throw new ResourceNotFoundException();
            }

            return NoContent();
        }

        [HttpPut("updateContactWithOrgAndStatus", Name = "UpdateContactWithOrgAndStatus")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateContactWithOrgAndStatus([FromBody] UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest)
        {
            _logger.LogInformation("Received Update Contact Status Request");

            await _mediator.Send(updateContactWithOrgAndStausRequest);

            return NoContent();
        }

        [PerformValidation]
        [HttpPost("callback", Name = "Callback")]
        public async Task<ActionResult> Callback([FromBody] SignInCallback callback)
        {
            _logger.LogInformation($"Received callback from DfE: Sub: {callback.Sub} SourceId: {callback.SourceId}");
            await _mediator.Send(new UpdateSignInIdRequest(Guid.Parse(callback.Sub), Guid.Parse(callback.SourceId)));
            await _mediator.Send(new InvitationCheckRequest(Guid.Parse(callback.SourceId)));
            return NoContent();
        }

        [HttpPost("createNewContactWithGivenId", Name = "CreateNewContactWithGivenId")]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(Contact))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<Contact>> CreateNewContactWithGivenId([FromBody] Contact contact)
        {
            _logger.LogInformation($"Creating a new contact only with given Id");
            var newContact = await _contactRepository.CreateNewContact(contact);
            return Ok(newContact);
        }

        [HttpPost("associateDefaultRolesAndPrivileges", Name = "AssociateDefaultRolesAndPrivileges")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> AssociateDefaultRolesAndPrivileges([FromBody] Contact contact)
        {
            _logger.LogInformation($"Associating roles and privileges to a contact");
            var privileges = await _contactQueryRepository.GetAllPrivileges();
            await _contactRepository.AssociatePrivilegesWithContact(contact.Id, privileges);
            return NoContent();
        }

        [HttpPost("setContactPrivileges")]
        public async Task<ActionResult> SetContactPrivileges([FromBody] SetContactPrivilegesRequest privilegesRequest)
        {
            var response = await _mediator.Send(privilegesRequest, CancellationToken.None);

            return Ok(response);
        }

        [HttpPost("removeContactFromOrganisation")]
        public async Task<ActionResult> RemoveContactFromOrganisation([FromBody] RemoveContactFromOrganisationRequest request)
        {
            return Ok(await _mediator.Send(request, CancellationToken.None));
        }

        [HttpPost("inviteContactToOrganisation")]
        public async Task<ActionResult> InviteContactToOrganisation([FromBody] InviteContactToOrganisationRequest request)
        {
            return Ok(await _mediator.Send(request, CancellationToken.None));
        }

        [HttpPost("requestForPrivilege")]
        public async Task<ActionResult> RequestForPrivilege([FromBody] RequestForPrivilegeRequest request)
        {
            await _mediator.Send(request, CancellationToken.None);
            return Ok();
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveContact([FromBody] ApproveContactRequest request)
        {
            await _mediator.Send(request, CancellationToken.None);
            return Ok();
        }

        [HttpPost("reject")]
        public async Task<ActionResult> RequestForPrivilege([FromBody] RejectContactRequest request)
        {
            await _mediator.Send(request, CancellationToken.None);
            return Ok();
        }
    }
}