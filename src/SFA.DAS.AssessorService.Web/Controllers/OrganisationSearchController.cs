using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class OrganisationSearchController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public OrganisationSearchController(IContactsApiClient contactsApiClient, IHttpContextAccessor contextAccessor)
        {
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Results(OrganisationSearchViewModel viewModel)
        {
            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);

            if (user.EndPointAssessorOrganisationId != null) { }

            //{
            //    return RedirectToAction("Applications", "Application");
            //}
            //else if (string.IsNullOrEmpty(viewModel.SearchString) || viewModel.SearchString.Length < 2)
            //{
            //    ModelState.AddModelError(nameof(viewModel.SearchString), "Enter a valid search string");
            //    TempData["ShowErrors"] = true;
            //    return RedirectToAction(nameof(Index));
            //}

            //viewModel.Organisations = await _apiClient.SearchOrganisation(viewModel.SearchString);

            if (string.IsNullOrEmpty(viewModel.SearchString) || viewModel.SearchString.Length < 2)
            {
                ModelState.AddModelError(nameof(viewModel.SearchString), "Enter a valid search string");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

    }
}