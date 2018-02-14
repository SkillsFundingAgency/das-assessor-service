namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AssessorService.Application.Interfaces;

    [Authorize]
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
        public async Task<IActionResult> Get(int ukprn)
        {
            var organisation = await _organisationRepository.GetByUkPrn(ukprn);
            if (organisation == null)
                return NotFound("No provider with ukprn {ukprn} found");

            return Ok(organisation);
        }
    }
}