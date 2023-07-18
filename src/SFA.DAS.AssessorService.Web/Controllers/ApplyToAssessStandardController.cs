using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ApplyForStandard)]
    [CheckSession]
    public class ApplyToAssessStandardController : Controller
    {
        public ApplyToAssessStandardController()
        {
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpGet]
        [Route("GoToApplyToAssessStandard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public IActionResult GoToApplyToAssessStandard()
        {
            return RedirectToAction("Index", "Dashboard");

            return Redirect($"/Application");
        }
    }
}