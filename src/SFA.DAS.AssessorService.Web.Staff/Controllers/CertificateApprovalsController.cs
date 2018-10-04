using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;
using AutoMapper;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Policy = Startup.Policies.OperationsTeamOnly)]
    public class CertificateApprovalsController : CertificateBaseController
    {
        public CertificateApprovalsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Approvals()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();            
            var certificatesToBeApproved = Mapper.Map<List<CertificateApprovalViewModel>>(certificates);
            
            return View(certificatesToBeApproved);
        }
    }
}