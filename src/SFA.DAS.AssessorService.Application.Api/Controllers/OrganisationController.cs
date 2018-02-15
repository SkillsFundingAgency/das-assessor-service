namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationController(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        [HttpGet("{ukprn}")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> Get(int ukprn)
        {
            var validator = new UkPrnValidator();
            var result = validator.Validate(ukprn);

            if (!result.IsValid)
                return BadRequest();

            var organisation = await _organisationRepository.GetByUkPrn(ukprn);
            if (organisation == null)
                return NotFound("No provider with ukprn {ukprn} found");

            return Ok(organisation);
        }
    }
}