﻿using System.Collections.Generic;
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
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IStandardVersionClient _standardVersionClient;
        private readonly ISessionService _sessionService;

        public CertificateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            IStandardVersionClient standardVersionClient,
            ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _standardVersionClient = standardVersionClient;
            _sessionService = sessionService;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateStartViewModel vm)
        {
            _sessionService.Remove(nameof(CertificateSession));
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            string singleVersionStandardUid = null;
            string singleOption = null;
            List<string> options = null;
            var versions = await _standardVersionClient.GetStandardVersionsByLarsCode(vm.StdCode);

            if (versions.Count() == 1)
            {
                singleVersionStandardUid = versions.Single().StandardUId;
                var optionsResult = await _standardVersionClient.GetStandardOptions(singleVersionStandardUid);

                if (optionsResult != null && optionsResult.HasOptions())
                {
                    if (optionsResult.OnlyOneOption())
                    {
                        singleOption = optionsResult.CourseOption.Single();
                    }

                    options = optionsResult.CourseOption.ToList();
                }
            }

            _logger.LogInformation(
                $"Start of Certificate Start for ULN {vm.Uln} and Standard Code: {vm.StdCode} by user {username}");

            var cert = await _certificateApiClient.Start(new StartCertificateRequest()
            {
                UkPrn = int.Parse(ukprn),
                StandardCode = vm.StdCode,
                Uln = vm.Uln,
                Username = username,
                StandardUId = singleVersionStandardUid,
                CourseOption = singleOption
            });

            var certificateSession = new CertificateSession()
            {
                CertificateId = cert.Id,
                Uln = vm.Uln,
                StandardCode = vm.StdCode,
                StandardUId = singleVersionStandardUid,
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
            else if (options != null)
            {
                return RedirectToAction("Option", "CertificateOption");
            }

            return RedirectToAction("Declare", "CertificateDeclaration");
        }
    }
}