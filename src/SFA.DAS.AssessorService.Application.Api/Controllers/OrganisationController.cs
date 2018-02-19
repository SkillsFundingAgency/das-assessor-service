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
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(IMediator mediator, 
            IOrganisationRepository organisationRepository,
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<OrganisationController> logger
            )
        {
            _mediator = mediator;
            _organisationRepository = organisationRepository;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("{ukprn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> Get(int ukprn)
        {
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            var organisation = await _organisationRepository.GetByUkPrn(ukprn);
            if (organisation == null)
            {
                return NotFound(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);
            }

            return Ok(organisation);
        }

        [HttpPost(Name = "Create")]
        [ValidateBadRequest]
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
    }
}