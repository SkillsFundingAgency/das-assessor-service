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
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
    [CheckSession]
    public class ApplyForOrganisationWithdrawal : Controller
    {
        public ApplyForOrganisationWithdrawal()
        {
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GoToApplyForOrganisationWithdrawal")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public IActionResult GoToApplyForOrganisationWithdrawal()
        {
            //return Redirect($"/OrganisationWithdrawalApplication");
            return RedirectToAction(nameof(ApplicationController.OrganisationWithdrawalApplications), nameof(ApplicationController).RemoveController());
        }
    }
}