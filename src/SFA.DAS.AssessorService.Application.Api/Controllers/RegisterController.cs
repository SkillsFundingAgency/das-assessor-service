﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{

    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/assessment-organisations")]
    [ValidateBadRequest]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator, ILogger<RegisterController> logger)
        {
            _mediator = mediator;
            _logger = logger;

        }

        [HttpPost(Name = "CreateEpaOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateEpaOrganisationRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationResponse(result));
            }
            
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation [{ex.Message}]");
                return Conflict(new EpaOrganisationResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut(Name = "UpdateEpaOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Gone, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateEpaOrganisation([FromBody] UpdateEpaOrganisationRequest request)
        {
            try
            {
                _logger.LogInformation($@"Updating Organisation [{request.OrganisationId}]");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationResponse(result));
            }

            catch (NotFoundException ex)
            {
                _logger.LogError($@"Record is not available for organisation ID: [{request.OrganisationId}]");
                return NotFound(new EpaOrganisationResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("contacts", Name = "CreateEpaOrganisationContact")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(EpaContact))]
        public async Task<IActionResult> CreateOrganisationContact([FromBody] CreateEpaOrganisationContactRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation Contact");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationContactResponse(result));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/email [{request.EndPointAssessorOrganisationId}, {request.Email}]");
                return Conflict(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("contacts", Name = "UpdateEpaOrganisationContact")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaContact))]
        public async Task<IActionResult> UpdateOrganisationContact([FromBody] UpdateEpaOrganisationContactRequest request)
        {
            try
            {
                _logger.LogInformation("Updating Organisation Contact");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationContactResponse(result));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for using this email in another organisation [{request.Email}]");
                return Conflict(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("contacts/associate-organisation", Name = "AssociateEpaContactWithOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IActionResult> AssociateEpaContactWithOrganisation([FromBody] AssociateEpaOrganisationWithEpaContactRequest request)
        {
            try
            {
                _logger.LogInformation($"Associating with Organisation [{request.OrganisationId}, {request.ContactId}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("update-primary-contact", Name = "UpdateEpaOrganisationPrimaryContact")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        public async Task<IActionResult> UpdateEpaOrganisationPrimaryContact([FromBody] UpdateEpaOrganisationPrimaryContactRequest request)
        {
            try
            {
                _logger.LogInformation($"Amending the Organisation [{request.OrganisationId} with Primary Contact {request.PrimaryContactId}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("update-phone-number", Name = "UpdateEpaOrganisationPhoneNumber")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        public async Task<IActionResult> UpdateEpaOrganisationPhoneNumber([FromBody] UpdateEpaOrganisationPhoneNumberRequest request)
        {
            try
            {
                _logger.LogInformation($"Amending the Organisation [{request.OrganisationId} with Phone Number {request.PhoneNumber}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("update-address", Name = "UpdateEpaOrganisationAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        public async Task<IActionResult> UpdateEpaOrganisationAddress([FromBody] UpdateEpaOrganisationAddressRequest request)
        {
            try
            {
                _logger.LogInformation($"Amending the Organisation [{request.OrganisationId} with Address {request.AddressLine1}, {request.AddressLine2}, {request.AddressLine3}, {request.AddressLine4}, {request.Postcode}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("update-email", Name = "UpdateEpaOrganisationEmail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        public async Task<IActionResult> UpdateEpaOrganisationEmail([FromBody] UpdateEpaOrganisationEmailRequest request)
        {
            try
            {
                _logger.LogInformation($"Amending the Organisation [{request.OrganisationId} with Email {request.Email}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("update-website-link", Name = "UpdateEpaOrganisationWebsiteLink")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        public async Task<IActionResult> UpdateEpaOrganisationWebsiteLink([FromBody] UpdateEpaOrganisationWebsiteLinkRequest request)
        {
            try
            {
                _logger.LogInformation($"Amending the Organisation [{request.OrganisationId} with Website Link {request.WebsiteLink}]");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("standards",Name = "CreateEpaOrganisationStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationStandard))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisationStandard([FromBody] CreateEpaOrganisationStandardRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation Standard");
                var result = await _mediator.Send(request);
                return Ok(new EpaoStandardResponse(result));
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($@"Record is not available for organisation / standard: [{request.OrganisationId}, {request.StandardCode}]");
                return NotFound(new EpaoStandardResponse(ex.Message));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/standard [{request.OrganisationId}, {request.StandardCode}]");
                return Conflict(new EpaoStandardResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaoStandardResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("standards", Name = "UpdateEpaOrganisationStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationStandard))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisationStandard([FromBody] UpdateEpaOrganisationStandardRequest request)
        {
            try
            {
                _logger.LogInformation($@"Updating Organisation Standard [{request.OrganisationId}, {request.StandardCode}]");
                var result = await _mediator.Send(request);
                return Ok(new EpaoStandardResponse(result));
            }      
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(new EpaoStandardResponse(ex.Message));
            }
        }

        [HttpPost("update-financials")]
        public async Task<IActionResult> UpdateOrganisationFinancials([FromBody] UpdateFinancialsRequest request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
