namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Middleware;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/organisations")]
    public class OrganisationQueryController : Controller
    {       
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<OrganisationQueryController> _logger;

        public OrganisationQueryController(
            IOrganisationQueryRepository organisationQueryRepository,
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<OrganisationQueryController> logger
            )
        {
            _organisationQueryRepository = organisationQueryRepository;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

        [HttpGet("{ukprn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(int ukprn)
        {         
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            var organisation = await _organisationQueryRepository.GetByUkPrn(ukprn);
            if (organisation == null)
            {
                return NotFound(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);
            }

            return Ok(organisation);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<OrganisationQueryViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get()
        {
            var organisations = await _organisationQueryRepository.GetAllOrganisations();
            return Ok(organisations);
        }
    }
}