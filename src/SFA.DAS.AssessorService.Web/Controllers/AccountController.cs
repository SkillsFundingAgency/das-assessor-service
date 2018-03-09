using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Orchestrators;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILoginOrchestrator _loginOrchestrator;

        public AccountController(IHttpContextAccessor contextAccessor, ILoginOrchestrator loginOrchestrator)
        {
            _contextAccessor = contextAccessor;
            _loginOrchestrator = loginOrchestrator;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> PostSignIn()
        {
            var loginResult = await _loginOrchestrator.Login(_contextAccessor.HttpContext);
            switch (loginResult)
            {
                case LoginResult.Valid:
                    return RedirectToAction("Index", "Organisation");
                case LoginResult.NotRegistered:
                    return RedirectToAction("NotRegistered", "Home");
                case LoginResult.InvalidRole:
                    return RedirectToAction("InvalidRole", "Home");
                default:
                    throw new ApplicationException();
            }
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
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
    }
}