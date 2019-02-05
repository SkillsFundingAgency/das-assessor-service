using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class ManageUsersController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;

        public ManageUsersController(ISessionService sessionService, IContactsApiClient contactsApiClient,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient)
        {
            _sessionService = sessionService;
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            _sessionService.Set("CurrentPage", Pages.Organisations);
            var response = new List<ContactsWithRolesResponse>();
            try
            {
                var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
                var organisation = await _organisationsApiClient.Get(ukprn);
                if (organisation != null)
                 response = await _contactsApiClient.GetContactsWithRoles(organisation.EndPointAssessorOrganisationId);
               
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            
           
            return View(response);
        }
    }
}