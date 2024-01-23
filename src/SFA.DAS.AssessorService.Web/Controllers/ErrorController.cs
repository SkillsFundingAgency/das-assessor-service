using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
[Route("[controller]")]
public class ErrorController : Controller
{
    private readonly IContactsApiClient _contactsApiClient;
    private readonly IOrganisationsApiClient _organisationsApiClient;
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(IContactsApiClient contactsApiClient, IOrganisationsApiClient organisationsApiClient, ILogger<ErrorController> logger)
    {
        _contactsApiClient = contactsApiClient;
        _organisationsApiClient = organisationsApiClient;
        _logger = logger;
    }
    
    [HttpGet(template:"403")]
    public async Task<IActionResult> AccessDenied()
    {
        if (TempData.Keys.Contains(nameof(PrivilegeAuthorizationDeniedContext)))
        {
            var deniedContext = JsonConvert
                .DeserializeObject<PrivilegeAuthorizationDeniedContext>(TempData[nameof(PrivilegeAuthorizationDeniedContext)]
                .ToString());

            var userId = Guid.Parse(User.FindFirst("UserId").Value);
            var user = await _contactsApiClient.GetById(userId);
            OrganisationResponse organisation = null;
            try
            {
                organisation = await _organisationsApiClient.GetOrganisationByUserId(userId);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                if (user.OrganisationId == null && user.Status == ContactStatus.Live)
                {
                    return RedirectToAction("Index", "OrganisationSearch");
                }
            }

            if (user.OrganisationId != null && user.Status == ContactStatus.InvitePending)
            {
                return RedirectToAction("InvitePending", "Home");
            }

            if (organisation != null && organisation.Status == OrganisationStatus.Applying ||
                organisation.Status == OrganisationStatus.New)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var privilege = (await _contactsApiClient.GetPrivileges()).Single(p => p.Id == deniedContext.PrivilegeId);

            var usersPrivileges = await _contactsApiClient.GetContactPrivileges(userId);
            
            return View("~/Views/Account/AccessDeniedForPrivilege.cshtml", new AccessDeniedViewModel
            {
                Title = privilege.UserPrivilege,
                Rights = privilege.PrivilegeData.Rights,
                PrivilegeId = deniedContext.PrivilegeId,
                ContactId = userId,
                UserHasUserManagement = usersPrivileges.Any(up => up.Privilege.Key == Privileges.ManageUsers),
                ReturnController = deniedContext.Controller,
                ReturnAction = deniedContext.Action,
                ReturnRouteName = deniedContext.RouteName,
                ReturnRouteValues = deniedContext.RouteValues.ToDictionary(item => item.Key, item => item.Value?.ToString()),
                IsUsersOrganisationLive = organisation?.Status == OrganisationStatus.Live
            });
        }
        else if (TempData.Keys.Contains("UnavailableFeatureContext"))
        {
            return View("~/Views/Account/UnavailableFeature.cshtml");
        }

        return View("~/Views/Account/AccessDenied.cshtml");
    }
}