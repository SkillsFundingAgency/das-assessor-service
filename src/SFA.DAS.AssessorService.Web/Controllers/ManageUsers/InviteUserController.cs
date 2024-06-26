using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [CheckSession]
    public class InviteUserController : ManageUsersBaseController
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IOrganisationsApiClient _organisationsApiClient;

        public InviteUserController(IContactsApiClient contactsApiClient, IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient) :base(contactsApiClient, contextAccessor)
        {
            _contactsApiClient = contactsApiClient;
            _organisationsApiClient = organisationsApiClient;
        }

        [HttpGet("/ManageUsers/Invite")]
        public async Task<IActionResult> Invite(string backController = "ManageUsers", string backAction = "Index")
        {
            var privileges = await _contactsApiClient.GetPrivileges();

            var vm = new InviteContactViewModel
            {
                PrivilegesViewModel = new EditPrivilegesViewModel
                {
                    PrivilegeViewModels = privileges.Select(p => new PrivilegeViewModel {Privilege = p}).ToArray()
                },
                BackController = backController,
                BackAction = backAction
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
                InvitedByContactId = (await GetRequestingContact()).Id,
                FamilyName = vm.FamilyName,
                GivenName = vm.GivenName,
                Email = vm.Email,
                OrganisationId = requestingContact.OrganisationId.GetValueOrDefault(),
                Privileges = vm.PrivilegesViewModel.PrivilegeViewModels.Where(pvm => pvm.Selected).Select(pvm => pvm.Privilege.Id).ToArray()
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
        public async Task<IActionResult> Invited(Guid contactId)
        {
            var contact = await _contactsApiClient.GetById(contactId);
            var organisation = await _organisationsApiClient.GetOrganisationByUserId(contactId);
            
            return View("~/Views/ManageUsers/InviteUser/Invited.cshtml", new InvitedViewModel {Email = contact.Email, Organisation = organisation.EndPointAssessorName});
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