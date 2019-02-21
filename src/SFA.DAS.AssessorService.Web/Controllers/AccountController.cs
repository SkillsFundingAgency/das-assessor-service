using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ILoginOrchestrator _loginOrchestrator;
        private readonly ISessionService _sessionService;

        public AccountController(ILogger<AccountController> logger, ILoginOrchestrator loginOrchestrator, ISessionService sessionService)
        {
            _logger = logger;
            _loginOrchestrator = loginOrchestrator;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
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
                    return RedirectToAction("NotRegistered", "Home");
                case LoginResult.InvalidRole:
                    return RedirectToAction("InvalidRole", "Home");
                case LoginResult.InvitePending:
                    ResetCookies();
                    _sessionService.Set("OrganisationName", loginResult.OrganisationName);
                    return RedirectToAction("InvitePending", "Home");
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
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);

            ResetCookies();
            
            return SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme,OpenIdConnectDefaults.AuthenticationScheme);
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
    }
}