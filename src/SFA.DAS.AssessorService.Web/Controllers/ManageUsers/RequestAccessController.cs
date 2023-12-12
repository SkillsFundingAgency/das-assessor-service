using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Account;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    public class RequestAccessController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;

        public RequestAccessController(IContactsApiClient contactsApiClient)
        {
            _contactsApiClient = contactsApiClient;
        }
        
        [HttpPost]
        public async Task<IActionResult> RequestAccess(AccessDeniedViewModel vm)
        {
            await _contactsApiClient.RequestForPrivilege(vm.ContactId, vm.PrivilegeId);

            if(vm.UserHasUserManagement)
            {
                if(!string.IsNullOrEmpty(vm.ReturnRouteName))
                {
                    return RedirectToRoute(vm.ReturnRouteName, vm.ReturnRouteValues);
                }

                return RedirectToAction(vm.ReturnAction, vm.ReturnController, vm.ReturnRouteValues);
            }

            return RedirectToAction("RequestSent", new { privilegeId = vm.PrivilegeId });
        }

        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Dashboard})]
        [HttpGet("/RequestAccess/RequestSent/{privilegeId}")]
        public async Task<IActionResult> RequestSent(Guid privilegeId)
        {
            var privilege = (await _contactsApiClient.GetPrivileges()).Single(p => p.Id == privilegeId); 
            
            return View("~/Views/Account/RequestSent.cshtml", privilege.UserPrivilege);
        }
    }
}