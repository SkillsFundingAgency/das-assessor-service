﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService, ICache cache, 
            IHttpContextAccessor contextAccessor, ILogger<OrganisationController> logger) : base(cache, contextAccessor, logger)
        {
            _organisationService = organisationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jwt = GetJwt();

            var organisation = await _organisationService.GetOrganisation(jwt);
            
            return View(organisation);
        }
    }
}