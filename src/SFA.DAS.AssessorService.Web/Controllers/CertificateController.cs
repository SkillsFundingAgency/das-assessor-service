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
using SFA.DAS.AssessorService.Web.Infrastructure;
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
        private readonly IMapper _mapper;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IStandardVersionClient _standardVersionClient;
        private readonly ISessionService _sessionService;
        private readonly IApprovalsLearnerApiClient _learnerApiClient;

        public CertificateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, IMapper mapper,
            ICertificateApiClient certificateApiClient,
            IStandardVersionClient standardVersionClient,
            ISessionService sessionService,
            IApprovalsLearnerApiClient learnerApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _certificateApiClient = certificateApiClient;
            _standardVersionClient = standardVersionClient;
            _sessionService = sessionService;
            _learnerApiClient = learnerApiClient;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateStartViewModel vm)
        {
            _sessionService.Remove(nameof(CertificateSession));

            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
           
            List<string> options = new List<string>();
            var singleOption = string.Empty;
            List<StandardVersion> versions = new List<StandardVersion>();

            var startCertificateRequest = new StartCertificateRequest()
            {
                UkPrn = int.Parse(ukprn),
                StandardCode = vm.StdCode,
                Uln = vm.Uln,
                Username = username
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

            var learner = await _learnerApiClient.GetLearnerRecord(vm.StdCode, vm.Uln);

            var versionsResult = await _standardVersionClient.GetStandardVersionsByLarsCode(vm.StdCode);

            if (learner.VersionConfirmed && !string.IsNullOrEmpty(learner.Version))
            {
                var version = versionsResult.First(v => v.Version == learner.Version);

                startCertificateRequest.StandardUId = version.StandardUId;
                versions.Add(version);

                if (!string.IsNullOrEmpty(learner.CourseOption))
                {
                    options.Add(learner.CourseOption);
                    startCertificateRequest.CourseOption = learner.CourseOption;
                }
                else
                {
                    await RetrieveAndPopulateStandardOptions(version.StandardUId);
                }
            }
            else
            {
                versions = versionsResult.ToList();

                if (versionsResult.Count() == 1) 
                {
                    startCertificateRequest.StandardUId = versionsResult.First().StandardUId;

                    await RetrieveAndPopulateStandardOptions(versionsResult.First().StandardUId);
                }
            }

            _logger.LogInformation($"Start of Certificate Start for ULN {vm.Uln} and Standard Code: {vm.StdCode} by user {username}");

            var cert = await _certificateApiClient.Start(startCertificateRequest);

            var certificateSession = new CertificateSession()
            {
                CertificateId = cert.Id,
                Uln = vm.Uln,
                StandardCode = vm.StdCode,
                StandardUId = startCertificateRequest.StandardUId,
                Versions = _mapper.Map<List<StandardVersionViewModel>>(versions),
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