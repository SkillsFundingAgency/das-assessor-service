using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
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
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IWebConfiguration _config;
        private readonly CreateAccountValidator _createAccountValidator;

        public AccountController(ILogger<AccountController> logger, ILoginOrchestrator loginOrchestrator,
            ISessionService sessionService, IWebConfiguration config, IContactsApiClient contactsApiClient, CreateAccountValidator createAccountValidator)
        {
            _logger = logger;
            _loginOrchestrator = loginOrchestrator;
            _sessionService = sessionService;
            _config = config;
            _contactsApiClient = contactsApiClient;
            _createAccountValidator = createAccountValidator;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties {RedirectUri = redirectUrl},
                "oidc");
        }

        [HttpGet]
        public async Task<IActionResult> PostSignIn()
        { 
            var loginResult = await _loginOrchestrator.Login();
            switch (loginResult.Result)
            {
                case LoginResult.Valid:
                    _sessionService.Set("OrganisationName", loginResult.OrganisationName);
                    return RedirectToAction("Index", "Dashboard");
                case LoginResult.NotRegistered:
                    return RedirectToAction("Index", "OrganisationSearch");
                case LoginResult.InvalidRole:
                    return RedirectToAction("InvalidRole", "Home");
                case LoginResult.InvitePending:
                    ResetCookies();
                    _sessionService.Set("OrganisationName", loginResult.OrganisationName);
                    return RedirectToAction("InvitePending", "Home");
                case LoginResult.Applying:
                    return Redirect($"{_config.ApplyBaseAddress}/Applications");
                case LoginResult.Rejected:
                    ResetCookies();
                    _sessionService.Set("OrganisationName", loginResult.OrganisationName);
                    return RedirectToAction("Rejected", "Home");
                default:
                    throw new ApplicationException();
            }
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            ResetCookies();

            return SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
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
        public IActionResult AccessDenied()
        {
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

            TempData["NewAccount"] = JsonConvert.SerializeObject(vm);

            return inviteSuccess.Result ? RedirectToAction("InviteSent") : RedirectToAction("Error", "Home");
            
        }
        [HttpGet]
        public IActionResult InviteSent()
        {
            CreateAccountViewModel viewModel;
            if (TempData["NewAccount"] is null)
            {
                viewModel = new CreateAccountViewModel() { Email = "[email placeholder]" };
            }
            else
            {
                viewModel = JsonConvert.DeserializeObject<CreateAccountViewModel>(TempData["NewAccount"].ToString());
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] DfeSignInCallback callback)
        {
            await _contactsApiClient.Callback(callback);
            return Ok();
        }
    }
}