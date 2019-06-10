using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    public class RequestAccessController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;

        public RequestAccessController(IContactsApiClient contactsApiClient)
        {
            _contactsApiClient = contactsApiClient;
        }
        
        [HttpPost]
        public async Task<IActionResult> Request(AccessDeniedViewModel vm)
        {
            await _contactsApiClient.RequestForPrivilege(vm.ContactId, vm.PrivilegeId);

            return vm.UserHasUserManagement 
                ? RedirectToAction(vm.ReturnAction, vm.ReturnController) 
                : RedirectToAction("RequestSent", new {privilegeId = vm.PrivilegeId});
        }

        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Organisations})]
        [HttpGet("/RequestAccess/RequestSent/{privilegeId}")]
        public async Task<IActionResult> RequestSent(Guid privilegeId)
        {
            var privilege = (await _contactsApiClient.GetPrivileges()).Single(p => p.Id == privilegeId); 
            
            return View("~/Views/Account/RequestSent.cshtml", privilege.UserPrivilege);
        }
    }
}