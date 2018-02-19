namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Attributes;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(IMediator mediator,
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<OrganisationController> logger
            )
        {
            _mediator = mediator;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

        [HttpPost(Name = "Create")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> Create(int ukprn,
            [FromBody] OrganisationCreateViewModel organisationCreateViewModel)
        {
            _logger.LogInformation("Received Create Request");

            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            var organisationQueryViewModel = await _mediator.Send(organisationCreateViewModel);

            return CreatedAtRoute("Create",
                new { ukprn = ukprn },
                organisationQueryViewModel);
        }

        [HttpPut(Name = "Update")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> Update(int ukprn,
          [FromBody] OrganisationUpdateViewModel organisationUpdateViewModel)
        {
            _logger.LogInformation("Received Update Request");

            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            var organisationQueryViewModel = await _mediator.Send(organisationUpdateViewModel);

            return NoContent();
        }

        [HttpDelete(Name = "Delete")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int ukprn)
        {
            try
            {
                _logger.LogInformation("Received Update Request");

                var result = _ukPrnValidator.Validate(ukprn);
                if (!result.IsValid)
                    return BadRequest(result.Errors[0].ErrorMessage);

                var organisationDeleteViewModel = new OrganisationDeleteViewModel
                {
                    UKPrn = ukprn
                };

                await _mediator.Send(organisationDeleteViewModel);

                return NoContent();
            }
            catch (NotFound exception)
            {             
                return NotFound();
            }
        }
    }
}