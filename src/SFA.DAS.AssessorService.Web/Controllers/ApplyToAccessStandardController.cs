using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class ApplyToAccessStandardController : Controller
    {

        private readonly IWebConfiguration _webConfiguration;
        private readonly IContactApplyClient _contactApplyContact;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;

        public ApplyToAccessStandardController(IWebConfiguration webConfiguration, IContactApplyClient contactApplyContact, 
            IHttpContextAccessor contextAccessor, IContactsApiClient contactsApiClient, IOrganisationsApiClient organisationsApiClient)
        {
            _webConfiguration = webConfiguration;
            _contactApplyContact = contactApplyContact;
            _contextAccessor = contextAccessor;
            _contactsApiClient = contactsApiClient;
            _organisationsApiClient = organisationsApiClient;
        }

        [HttpGet]
        [Route("ApplyToAccessStandard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GoToApplyToAccessStandard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public async Task<IActionResult> GoToApplyToAccessStandard()
        {
            var signinId = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var applyContact = await _contactApplyContact.GetApplyContactBySignInId(Guid.Parse(signinId));
            if (applyContact == null)
            {
                await _contactsApiClient.MigrateSingleContactToApply(Guid.Parse(signinId));
            }
            
            return Redirect($"{_webConfiguration.ApplyBaseAddress}/Applications");
        }
    }
}