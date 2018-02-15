namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;

        public OrganisationController(IOrganisationRepository organisationRepository,
            IStringLocalizer<OrganisationController> localizer)
        {
            _organisationRepository = organisationRepository;
            _localizer = localizer;
        }

        [HttpGet]
        [HttpGet("{ukprn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> Get(int ukprn)
        {
            var validator = new UkPrnValidator();
            var result = validator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest();

            var organisation = await _organisationRepository.GetByUkPrn(ukprn);
            if (organisation == null)
            {
                return NotFound(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);                
            }

            return Ok(organisation);
        }
    }
}