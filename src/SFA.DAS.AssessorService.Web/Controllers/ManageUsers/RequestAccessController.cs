using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
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
                : RedirectToAction("RequestSent");
        }

        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Organisations})]
        public IActionResult RequestSent()
        {
            return View("~/Views/Account/RequestSent.cshtml");
        }
    }
}