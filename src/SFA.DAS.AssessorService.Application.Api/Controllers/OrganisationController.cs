using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationRepository _organisationRepository;
        
        public OrganisationController(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }


        [HttpGet]
        // GET
        public async Task<IActionResult> Get()
        {
            var ukprn = (User.FindFirst("ukprn"))?.Value;
            var organisation = await _organisationRepository.GetByUkPrn(ukprn);

            return Ok(organisation);
        }
    }
}