using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/organisations")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly ApiClient _apiClient;

        public OrganisationController(ILogger<OrganisationController> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        // GET api/organisations
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<OrganisationResponse> organisations = await _apiClient.GetAllOrganisations();

            return Ok(organisations);
        }
    }
}