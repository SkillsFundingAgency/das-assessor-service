using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ILoginOrchestrator _loginOrchestrator;
        private readonly ISessionService _sessionService;
        private readonly IWebConfiguration _config;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly CreateAccountValidator _createAccountValidator;
        private readonly UpdateAccountValidator _updateAccountValidator;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, ILoginOrchestrator loginOrchestrator,
            ISessionService sessionService, IWebConfiguration config, IContactsApiClient contactsApiClient,
            IHttpContextAccessor contextAccessor, CreateAccountValidator createAccountValidator, IOrganisationsApiClient organisationsApiClient,  UpdateAccountValidator updateAccountValidator, IConfiguration configuration)
        {
            _logger = logger;
            _loginOrchestrator = loginOrchestrator;
            _sessionService = sessionService;
            _config = config;
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
            _createAccountValidator = createAccountValidator;
            _organisationsApiClient = organisationsApiClient;
            _updateAccountValidator = updateAccountValidator;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            
            _ = bool.TryParse(_configuration["StubAuth"], out var stubAuth);
            var authenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme;
            if (stubAuth)
            {
                authenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
            }

            
            return Challenge(
                new AuthenticationProperties {RedirectUri = redirectUrl},
                authenticationSchemes);
        }

        [HttpGet]
        public async Task<IActionResult> PostSignIn()
        {
            var loginResult = await _loginOrchestrator.Login();
            //            var orgName = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/orgname")?.Value;
            var epaoId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            _logger.LogInformation($"  returned from LoginOrchestrator: {loginResult.Result}");

            switch (loginResult.Result)
            {
                case LoginResult.Valid:

                    _sessionService.Set("EndPointAssessorOrganisationId", epaoId);
                    return RedirectToAction("Index", "Dashboard");
                case LoginResult.NotRegistered:
                    return RedirectToAction("Index", "OrganisationSearch");
                case LoginResult.NotActivated:
                    _sessionService.Set("EndPointAssessorOrganisationId", loginResult.EndPointAssessorOrganisationId);
                    return RedirectToAction("NotActivated", "Home");
                case LoginResult.InvalidRole:
                    return RedirectToAction("InvalidRole", "Home");
                case LoginResult.InvitePending:
                    //ResetCookies();
                    _sessionService.Set("EndPointAssessorOrganisationId", epaoId);
                    return RedirectToAction("InvitePending", "Home");
                case LoginResult.Applying:
                    return RedirectToAction("Applications", "Application");
                case LoginResult.Rejected:
                    ResetCookies();
                    _sessionService.Set("EndPointAssessorOrganisationId", epaoId);
                    return RedirectToAction("Rejected", "Home");
                case LoginResult.ContactDoesNotExist:
                    return RedirectToAction("UpdateAnAccount", "Account");
                default:
                    throw new ApplicationException();
            }
        }

        [HttpGet]
        public new async Task<IActionResult> SignOut()
        {
            ResetCookies();

            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token",idToken);

            var schemes = new List<string>
            {
                CookieAuthenticationDefaults.AuthenticationScheme
            };
            _ = bool.TryParse(_configuration["StubAuth"], out var stubAuth);
            if (!stubAuth)
            {
                schemes.Add(OpenIdConnectDefaults.AuthenticationScheme);
            }
        
            return SignOut(
                authenticationProperties, 
                schemes.ToArray());
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [HttpGet]
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

                if (organisation != null)
                {
                    if (organisation.Status == OrganisationStatus.Applying ||
                    organisation.Status == OrganisationStatus.New)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

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

            return View();
        }

        private void ResetCookies()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult UpdateAnAccount()
        {
            return View(new AccountViewModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAnAccount(AccountViewModel accountViewModel)
        {
            await _updateAccountValidator.ValidateAsync(accountViewModel);

            if (!ModelState.IsValid)
            {
                return View(accountViewModel);
            }

            var email = User.Identities.FirstOrDefault()?.FindFirst(ClaimTypes.Email)?.Value;
            var govIdentifier = User.Identities.FirstOrDefault()?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var inviteSuccess =
                await _contactsApiClient.InviteUser(new CreateContactRequest(accountViewModel.GivenName, accountViewModel.FamilyName, email, null, email, govIdentifier));

            return inviteSuccess.Result ? RedirectToAction("Index", "OrganisationSearch") : RedirectToAction("Error", "Home");
        }
        

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] SignInCallback callback)
        {
            await _contactsApiClient.Callback(callback);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangeSignInDetails()
        {
            var model = new ChangeSignInDetailsViewModel(_configuration["ResourceEnvironmentName"]);
            return View(model);
        }
    }
}