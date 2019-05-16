using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDetailsController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _contactsApiClient = contactsApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("/ManageUsers/{userid}")]
        public async Task<IActionResult> User(Guid userid)
        {
            // Get calling userId from claims
            // Get 
            
            
            var contact = await _contactsApiClient.GetById(userid.ToString());

            var vm = Mapper.Map<UserViewModel>(contact);

            vm.AssignedPrivileges = await _contactsApiClient.GetContactPrivileges(userid);
            
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