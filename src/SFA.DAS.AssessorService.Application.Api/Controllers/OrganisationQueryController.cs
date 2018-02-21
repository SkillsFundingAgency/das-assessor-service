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
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationQueryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<OrganisationQueryController> _logger;

        public OrganisationQueryController(IMediator mediator,
            IOrganisationRepository organisationRepository,
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<OrganisationQueryController> logger
            )
        {
            _mediator = mediator;
            _organisationRepository = organisationRepository;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

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

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> Get()
        {
            var organisations = await _organisationRepository.GetAllOrganisations();
            return Ok(organisations);
        }
    }
}