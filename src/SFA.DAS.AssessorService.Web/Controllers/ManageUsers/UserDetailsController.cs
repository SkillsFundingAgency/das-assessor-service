using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    [CheckSession]
    public class UserDetailsController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;

        public UserDetailsController(IContactsApiClient contactsApiClient)
        {
            _contactsApiClient = contactsApiClient;
        }

        [HttpGet("/ManageUsers/{userid}")]
        public async Task<IActionResult> User(Guid userid)
        {
            var contact = await _contactsApiClient.GetById(userid.ToString());

            var vm = Mapper.Map<UserViewModel>(contact);

            vm.AssignedPrivileges = await _contactsApiClient.GetContactPrivileges(userid);
            
            return View("~/Views/ManageUsers/UserDetails/User.cshtml", vm);
        }
    }
}