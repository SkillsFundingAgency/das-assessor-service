using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult EditPermissions(Guid userid)
        {
            throw new NotImplementedException();
        }

        [HttpGet("/ManageUsers/{userid}/remove")]
        public IActionResult Remove()
        {
            throw new NotImplementedException();
        }
    }
}