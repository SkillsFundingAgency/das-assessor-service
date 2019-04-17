using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(ILogger<OrganisationController> logger, IHttpContextAccessor contextAccessor, IOrganisationsApiClient apiClient)
        {
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            OrganisationResponse organisation;

            try
            {
                if (ukprn != null)
                    organisation = await _apiClient.Get(ukprn);
                else
                {
                    var epaOrganisation = await _apiClient.GetEpaOrganisation(epaoid);
                    organisation = new OrganisationResponse
                    {
                        EndPointAssessorName = epaOrganisation.Name,
                        EndPointAssessorOrganisationId = epaoid,
                        EndPointAssessorUkprn = null,
                        Id = epaOrganisation.Id,
                        PrimaryContact = epaOrganisation.PrimaryContact,
                        Status = epaOrganisation.Status
                    };
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return View(organisation);
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> OrganisationDetails()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _apiClient.GetEpaOrganisation(epaoid);
                var viewModel = MapOrganisationModel(organisation);
                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
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
                PrimaryContactName = !string.IsNullOrEmpty(organisation.PrimaryContactName)
                    ? organisation.PrimaryContactName
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