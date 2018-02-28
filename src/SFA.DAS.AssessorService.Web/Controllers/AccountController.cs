using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IWebConfiguration _config;
        private readonly IContactsApiClient _contactsApiClient;

        public AccountController(IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient, IWebConfiguration config, IContactsApiClient contactsApiClient)
        {
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _config = config;
            _contactsApiClient = contactsApiClient;
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
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
            var email = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/mail").Value;

            if (!_contextAccessor.HttpContext.User.HasClaim("http://schemas.portal.com/service",
                _config.Authentication.Role))
            {
                return RedirectToAction("InvalidRole", "Home");
            }
            else
            {
                Organisation organisation;
                try
                {
                    organisation = await _organisationsApiClient.Get(ukprn, ukprn);
                }
                catch (EntityNotFoundException)
                {
                    return RedirectToAction("NotRegistered", "Home");
                }

                Contact contact;
                try
                {
                    contact = await _contactsApiClient.GetByUsername(ukprn, username);
                }
                catch (EntityNotFoundException )
                {
                    // Contact not found in db, post a new one.
                    contact = await _contactsApiClient.Create(new Contact() {ContactEmail = email, })
                }

                return RedirectToAction("Index", "Organisation");
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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}