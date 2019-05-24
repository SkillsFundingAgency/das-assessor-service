using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    [CheckSession]
    public class InviteUserController : ManageUsersBaseController
    {
        private readonly IContactsApiClient _contactsApiClient;

        public InviteUserController(IContactsApiClient contactsApiClient, IHttpContextAccessor contextAccessor) :base(contactsApiClient, contextAccessor)
        {
            _contactsApiClient = contactsApiClient;
        }

        [HttpGet("/ManageUsers/Invite")]
        public async Task<IActionResult> Invite()
        {
            var privileges = await _contactsApiClient.GetPrivileges();

            var vm = new InviteContactViewModel
            {
                PrivilegesViewModel = new EditPrivilegesViewModel
                {
                    PrivilegeViewModels = privileges.Select(p => new PrivilegeViewModel {Privilege = p}).ToArray()
                }
            };


            return View("~/Views/ManageUsers/InviteUser/Invite.cshtml", vm);
        }

        [HttpPost("/ManageUsers/Invite")]
        public async Task<IActionResult> Invite(InviteContactViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await RebuildViewModel(vm);

                return View("~/Views/ManageUsers/InviteUser/Invite.cshtml", vm);
            }

            var requestingContact = await GetRequestingContact();

            var result = await _contactsApiClient.InviteContactToOrganisation(new InviteContactToOrganisationRequest
            {
                FamilyName = vm.FamilyName,
                GivenName = vm.GivenName,
                Email = vm.Email,
                OrganisationId = requestingContact.OrganisationId.GetValueOrDefault()
            });

            if (!result.Success)
            {
                ModelState.AddModelError("Email", result.ErrorMessage);

                await RebuildViewModel(vm);
                
                return View("~/Views/ManageUsers/InviteUser/Invite.cshtml", vm);
            }


            return RedirectToAction("Invited", new {result.ContactId});
        }

        [HttpGet("/ManageUsers/{contactId}/Invited")]
        public IActionResult Invited(Guid contactId)
        {
            return Ok();
        }
        
        private async Task RebuildViewModel(InviteContactViewModel vm)
        {
            var privileges = await _contactsApiClient.GetPrivileges();

            foreach (var privilegeViewModel in vm.PrivilegesViewModel.PrivilegeViewModels)
            {
                privilegeViewModel.Privilege = privileges.Single(p => p.Id == privilegeViewModel.Privilege.Id);
            }
        }

        
    }
}