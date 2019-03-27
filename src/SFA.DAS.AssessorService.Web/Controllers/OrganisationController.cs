﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class OrganisationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public OrganisationController(ILogger<OrganisationController> logger, IHttpContextAccessor contextAccessor, IOrganisationsApiClient apiClient, ISessionService sessionService)
        {
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn").Value;
            
            OrganisationResponse organisation;

            try
            {
                organisation = await _apiClient.Get(ukprn);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View(organisation);
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> OrganisationDetails()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn").Value;
            try
            {
                var organisationResponse = await _apiClient.Get(ukprn);
                var organisation = await _apiClient.GetEpaOrganisation(organisationResponse.EndPointAssessorOrganisationId);
                var viewModel = MapOrganisationModel(organisation);
                return View(viewModel);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        private ViewAndEditOrganisationViewModel MapOrganisationModel(EpaOrganisation organisation)
        {
            const string notSetDescription = "Not set";
            var viewModel = new ViewAndEditOrganisationViewModel
            {
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                Ukprn = organisation.Ukprn,
                OrganisationTypeId = organisation.OrganisationTypeId,
                OrganisationType = notSetDescription,
                LegalName = organisation.OrganisationData?.LegalName,
                TradingName = organisation.OrganisationData?.TradingName,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = !string.IsNullOrEmpty(organisation.PrimaryContact)
                    ? organisation.PrimaryContact
                    : notSetDescription,
                PrimaryContactName = !string.IsNullOrEmpty(organisation.PrimaryContact)
                    ? organisation.PrimaryContact
                    : notSetDescription,
                CharityNumber = organisation.OrganisationData?.CharityNumber,
                CompanyNumber = organisation.OrganisationData?.CompanyNumber,
                Status = organisation.Status,
                OrganisationTypes = _apiClient.GetOrganisationTypes().Result
            };


            if (viewModel.OrganisationTypeId == null) return viewModel;
            var organisationTypes = viewModel.OrganisationTypes;
            viewModel.OrganisationType =
                organisationTypes.FirstOrDefault(x => x.Id == viewModel.OrganisationTypeId)?.Type;

            return viewModel;
        }

    }
}