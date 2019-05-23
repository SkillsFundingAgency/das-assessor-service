using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    public class UserDetailsController : ManageUsersBaseController
    {
        public UserDetailsController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor) : base(contactsApiClient, httpContextAccessor){}

        [HttpGet("/ManageUsers/{contactId}")]
        public async Task<IActionResult> User(Guid contactId)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(contactId);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }

            var vm = Mapper.Map<UserViewModel>(securityCheckpoint.contact);

            vm.AssignedPrivileges = await ContactsApiClient.GetContactPrivileges(contactId);
            
            return View("~/Views/ManageUsers/UserDetails/User.cshtml", vm);
        }

        [HttpGet("/ManageUsers/{userid}/permissions")]
        public async Task<IActionResult> EditPermissions(Guid userid)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(userid);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }

            var vm = await GetUserViewModel(userid, securityCheckpoint);

            return View("~/Views/ManageUsers/UserDetails/EditUserPermissions.cshtml", vm);
        }

        private async Task<UserViewModel> GetUserViewModel(Guid userid, (bool isValid, ContactResponse contact) securityCheckpoint)
        {
            var vm = Mapper.Map<UserViewModel>(securityCheckpoint.contact);

            vm.AssignedPrivileges = await ContactsApiClient.GetContactPrivileges(userid);

            vm.AllPrivilegeTypes = await ContactsApiClient.GetPrivileges();
            return vm;
        }

        [HttpPost("/ManageUsers/{userid}/permissions")]
        public async Task<IActionResult> EditPermissions(EditPrivilegesViewModel vm)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(vm.ContactId);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }
            
            var response = await ContactsApiClient.SetContactPrivileges(
                new SetContactPrivilegesRequest()
                {
                    AmendingContactId = RequestingUser.Id,
                    ContactId = vm.ContactId, 
                    PrivilegeIds = vm.PrivilegeViewModels.Where(pvm => pvm.Selected).Select(pvm => pvm.Privilege.Id).ToArray()
                });

            if (!response.Success)
            {
                ModelState.AddModelError("permissions", response.ErrorMessage);
                
                var editVm = await GetUserViewModel(vm.ContactId, securityCheckpoint);
            
                return View("~/Views/ManageUsers/UserDetails/EditUserPermissions.cshtml", editVm);
            }
            
            return RedirectToAction("User", new {contactId = vm.ContactId});
        }

        [HttpGet("/ManageUsers/{contactId}/remove")]
        public async Task<IActionResult> Remove(Guid contactId)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(contactId);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }

            return View("~/Views/ManageUsers/UserDetails/RemoveConfirm.cshtml", UserToBeDisplayed);
        }

        [HttpPost("/ManageUsers/{contactId}/remove")]
        public async Task<IActionResult> RemoveConfirmed(Guid contactId)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(contactId);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }

            var response = await ContactsApiClient.RemoveContactFromOrganisation(RequestingUser.Id, contactId);

            if (!response.Success)
            {
                ModelState.AddModelError("permissions", response.ErrorMessage);
            
                return View("~/Views/ManageUsers/UserDetails/RemoveConfirm.cshtml", UserToBeDisplayed);
            }
            else
            {
                return response.SelfRemoved 
                    ? RedirectToAction("SignOut", "Account") 
                    : RedirectToAction("Index", "ManageUsers");
            }
        }
        
    }
}