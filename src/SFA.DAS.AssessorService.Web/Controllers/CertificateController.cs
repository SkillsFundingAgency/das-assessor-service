using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    [Route("certificate")]
    public class CertificateController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IStandardVersionClient _standardVersionClient;
        private readonly ISessionService _sessionService;
        private readonly ISearchOrchestrator _searchOrchestrator;

        public CertificateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            IStandardVersionClient standardVersionClient,
            ISessionService sessionService,
            ISearchOrchestrator searchOrchestrator)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _standardVersionClient = standardVersionClient;
            _sessionService = sessionService;
            _searchOrchestrator = searchOrchestrator;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateStartViewModel vm)
        {
            _sessionService.Remove(nameof(CertificateSession));
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            List<string> options = new List<string>();
            var singleOption = string.Empty;
            IEnumerable<StandardVersion> versions = new List<StandardVersion>();

            var startCertificateRequest = new StartCertificateRequest()
            {
                UkPrn = int.Parse(ukprn),
                StandardCode = vm.StdCode,
                Uln = vm.Uln,
                Username = username,
                StandardUId = vm.StandardUId,
                CourseOption = vm.Option
            };
            
            async Task RetrieveAndPopulateStandardOptions(string standardUId)
            {
                var optionsResult = await _standardVersionClient.GetStandardOptions(standardUId);

                if (optionsResult != null && optionsResult.HasOptions())
                {
                    if (optionsResult.OnlyOneOption())
                    {
                        singleOption = optionsResult.CourseOption.Single();
                        startCertificateRequest.CourseOption = singleOption;
                    }

                    options = optionsResult.CourseOption.ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(vm.FamilyName))
            {
                //This has come from the multiple standard choice selection page. Only the ULN and STD code are supplied.
                //Need to re-search for the apprenticeship, to determine of approvals data is available and if there is no need to show an option.
                var result = await _searchOrchestrator.Search(new ViewModels.Search.SearchRequestViewModel { Surname = vm.FamilyName, Uln = vm.Uln.ToString() });
                var relevantStandard = result.SearchResults.FirstOrDefault(s => s.StdCode == vm.StdCode.ToString());
                var isAFail = relevantStandard.OverallGrade == CertificateGrade.Fail && relevantStandard.SubmittedAt != null;
                if (relevantStandard.Versions != null && relevantStandard.Versions.Count() == 1 && !isAFail)
                {
                    var searchStandardVersion = relevantStandard.Versions.First();
                    startCertificateRequest.StandardUId = searchStandardVersion.StandardUId;

                    var standardVersion = await _standardVersionClient.GetStandardVersionById(searchStandardVersion.StandardUId);
                    versions = new List<StandardVersion> { standardVersion };
                    
                    if (searchStandardVersion.Options != null && searchStandardVersion.Options.Count() == 1)
                    {
                        var option = searchStandardVersion.Options.First();
                        startCertificateRequest.CourseOption = option;
                        options = new List<string> { option };
                    }
                    else
                    {
                        await RetrieveAndPopulateStandardOptions(searchStandardVersion.StandardUId);
                    }
                }
                else
                {
                    versions = await _standardVersionClient.GetStandardVersionsByLarsCode(vm.StdCode);
                }
            }
            else if (string.IsNullOrWhiteSpace(vm.StandardUId) || vm.SubmittedFail)
            {
                // StandardUid empty, need to navigate EPAO through version/option if applicable
                versions = await _standardVersionClient.GetStandardVersionsByLarsCode(vm.StdCode);

                if (versions.Count() == 1)
                {
                    var standardUId = versions.Single().StandardUId;
                    startCertificateRequest.StandardUId = standardUId;
                    await RetrieveAndPopulateStandardOptions(standardUId);
                }
            }
            //StandardUId populated, but option empty, check if options are required.
            else if (string.IsNullOrWhiteSpace(vm.Option))
            {
                // We have a version, which is confirmed, but we don't have an option
                // This could be a scenario where either the standard only has one version so is confirmed
                // But has no option which requires it ( could occurr from populate learner )
                // Or in commitments, the option was never specified "TBC" and now we need to set it
                // This is now on the EPAO to set.
                await RetrieveAndPopulateStandardOptions(vm.StandardUId);
            }
            else
            {
                // StandardUId is populated AND Option is populated
                var standardVersion = await _standardVersionClient.GetStandardVersionById(vm.StandardUId);
                options = new List<string> { vm.Option };
                versions = new List<StandardVersion> { standardVersion };
            }

            _logger.LogInformation(
                $"Start of Certificate Start for ULN {vm.Uln} and Standard Code: {vm.StdCode} by user {username}");

            var cert = await _certificateApiClient.Start(startCertificateRequest);

            var certificateSession = new CertificateSession()
            {
                CertificateId = cert.Id,
                Uln = vm.Uln,
                StandardCode = vm.StdCode,
                StandardUId = startCertificateRequest.StandardUId,
                Versions = Mapper.Map<List<StandardVersionViewModel>>(versions),
                Options = options
            };

            _sessionService.Set(nameof(CertificateSession), certificateSession);
            _logger.LogInformation($"New Certificate received for ULN {vm.Uln} and Standard Code: {vm.StdCode} with ID {cert.Id}");

            if (versions.Count() > 1)
            {
                return RedirectToAction("Version", "CertificateVersion");
            }
            else if (!string.IsNullOrWhiteSpace(singleOption))
            {
                return RedirectToAction("Declare", "CertificateDeclaration");
            }
            else if (options.Count > 1)
            {
                return RedirectToAction("Option", "CertificateOption");
            }

            return RedirectToAction("Declare", "CertificateDeclaration");
        }
    }
}