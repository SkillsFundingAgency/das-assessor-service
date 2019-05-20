using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    public class UserDetailsController : ManageUsersBaseController
    {
        public UserDetailsController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor) : base(contactsApiClient, httpContextAccessor){}

        [HttpGet("/ManageUsers/{userid}")]
        public async Task<IActionResult> User(Guid userid)
        {
            var securityCheckpoint = await SecurityCheckAndGetContact(userid);

            if (!securityCheckpoint.isValid)
            {
                return Unauthorized();
            }

            var vm = Mapper.Map<UserViewModel>(securityCheckpoint.contact);

            vm.AssignedPrivileges = await ContactsApiClient.GetContactPrivileges(userid);
            
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
                    ContactId = vm.ContactId, 
                    PrivilegeIds = vm.PrivilegeViewModels.Where(pvm => pvm.Selected).Select(pvm => pvm.Privilege.Id).ToArray()
                });

            if (!response.Success)
            {
                ModelState.AddModelError("permissions", response.ErrorMessage);
                
                var editVm = await GetUserViewModel(vm.ContactId, securityCheckpoint);
            
                return View("~/Views/ManageUsers/UserDetails/EditUserPermissions.cshtml", editVm);
            }
            
            return RedirectToAction("User", new {userId = vm.ContactId});
        }

        [HttpGet("/ManageUsers/{userid}/remove")]
        public IActionResult Remove()
        {
            throw new NotImplementedException();
        }
    }
}