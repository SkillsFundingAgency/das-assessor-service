namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Attributes;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/organisation")]
    public class OrganisationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(IMediator mediator,
            IStringLocalizer<OrganisationController> localizer,
            ILogger<OrganisationController> logger
            )
        {
            _mediator = mediator;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost(Name = "CreateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> CreateOrganisation([FromBody] OrganisationCreateViewModel organisationCreateViewModel)
        {
            _logger.LogInformation("Received Create Organisation Request");

            var organisationQueryViewModel = await _mediator.Send(organisationCreateViewModel);

            return CreatedAtRoute("CreateOrganisation",
                new { id = organisationQueryViewModel.Id },
                organisationQueryViewModel);
        }

        [HttpPut(Name = "UpdateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> UpdateOrganisation([FromBody] OrganisationUpdateViewModel organisationUpdateViewModel)
        {
            _logger.LogInformation("Received Update Organisation Request");

            var organisationQueryViewModel = await _mediator.Send(organisationUpdateViewModel);

            return NoContent();
        }

        [HttpDelete(Name = "DeleteOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteOrganisation(Guid id)
        {
            try
            {
                _logger.LogInformation("Received Delete Organisation Request");

                var organisationDeleteViewModel = new OrganisationDeleteViewModel
                {
                    Id = id
                };

                await _mediator.Send(organisationDeleteViewModel);

                return NoContent();
            }
            catch (NotFound)
            {
                return NotFound();
            }
        }
    }
}