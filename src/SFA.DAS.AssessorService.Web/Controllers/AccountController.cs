using System;
using System.Linq;
using System.Threading.Tasks;
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

        public AccountController(ILogger<AccountController> logger, ILoginOrchestrator loginOrchestrator,
            ISessionService sessionService, IWebConfiguration config, IContactsApiClient contactsApiClient,
            IHttpContextAccessor contextAccessor, CreateAccountValidator createAccountValidator, IOrganisationsApiClient organisationsApiClient)
        {
            _logger = logger;
            _loginOrchestrator = loginOrchestrator;
            _sessionService = sessionService;
            _config = config;
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
            _createAccountValidator = createAccountValidator;
            _organisationsApiClient = organisationsApiClient;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties {RedirectUri = redirectUrl},
                OpenIdConnectDefaults.AuthenticationScheme);
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
                    ResetCookies();
                    return RedirectToAction("NotRegistered", "Home");
                default:
                    throw new ApplicationException();
            }
        }

        [HttpGet]
        public new IActionResult SignOut()
        {
            ResetCookies();

            if(!User.Identity.IsAuthenticated)
            {
                // If they are no longer authenticated then the cookie has expired. Don't try to signout.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
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

        [HttpGet]
        public IActionResult CreateAnAccount()
        {
            var vm = new CreateAccountViewModel();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnAccount(CreateAccountViewModel vm)
        {

            _createAccountValidator.Validate(vm);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var inviteSuccess =
                await _contactsApiClient.InviteUser(new CreateContactRequest(vm.GivenName, vm.FamilyName, vm.Email,null,vm.Email));

            _sessionService.Set("NewAccount", JsonConvert.SerializeObject(vm));
            return inviteSuccess.Result ? RedirectToAction("InviteSent") : RedirectToAction("Error", "Home");
            
        }
        [HttpGet]
        public IActionResult InviteSent()
        {
            CreateAccountViewModel viewModel;
            var newAccount = _sessionService.Get("NewAccount");
            if (string.IsNullOrEmpty(newAccount))
            {
                viewModel = new CreateAccountViewModel() { Email = "[email placeholder]" };
            }
            else
            {
                viewModel = JsonConvert.DeserializeObject<CreateAccountViewModel>(newAccount);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] SignInCallback callback)
        {
            await _contactsApiClient.Callback(callback);
            return Ok();
        }

    }
}