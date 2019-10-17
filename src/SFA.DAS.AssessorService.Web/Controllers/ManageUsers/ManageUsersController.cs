using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [PrivilegeAuthorize(Privileges.ManageUsers)]
    [CheckSession]
    public class ManageUsersController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IEmailApiClient _emailApiClient;
        private readonly IWebConfiguration _config;

        public ManageUsersController(IWebConfiguration config, IContactsApiClient contactsApiClient,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient, IEmailApiClient emailApiClient)
        {
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _emailApiClient = emailApiClient;
            _config = config;
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Organisations})]
        public async Task<IActionResult> Index()
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst("UserId")?.Value;
            var organisation = await _organisationsApiClient.GetOrganisationByUserId(Guid.Parse(userId));

            var response = await _contactsApiClient.GetAllContactsForOrganisationIncludePrivileges(organisation.EndPointAssessorOrganisationId);

            return View(response);
        }

        [HttpGet]
        [Route("/[controller]/status/{id}/{status}")]
        public async Task<IActionResult> SetStatusAndNotify(Guid id, string status)
        {
            if (status == ContactStatus.Approve)
            {
                await _contactsApiClient.ApproveContact(id);
            }
            else
            {
                await _contactsApiClient.RejectContact(id);
            }


            return RedirectToAction("Index");
        }
    }
}