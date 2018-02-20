using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _apiClient;

        public AccountController(IHttpContextAccessor contextAccessor, IOrganisationsApiClient apiClient)
        {
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
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
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn").Value;
            var organisationExists = await _apiClient.Get(ukprn, ukprn);
            return organisationExists != null ? this.RedirectToAction("Index", "Organisation") : this.RedirectToAction("NotRegistered", "Home");
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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}