using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class OrganisationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IEmailApiClient _emailApiClient;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(ILogger<OrganisationController> logger, IHttpContextAccessor contextAccessor, 
            IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient,
            IEmailApiClient emailApiClient)
        {
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
            _emailApiClient = emailApiClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            OrganisationResponse organisation;

            try
            {
                if (ukprn != null)
                    organisation = await _organisationsApiClient.Get(ukprn);
                else
                {
                    var epaOrganisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                    organisation = new OrganisationResponse
                    {
                        EndPointAssessorName = epaOrganisation.Name,
                        EndPointAssessorOrganisationId = epaoid,
                        EndPointAssessorUkprn = null,
                        Id = epaOrganisation.Id,
                        PrimaryContact = epaOrganisation.PrimaryContact,
                        Status = epaOrganisation.Status
                    };
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return View(organisation);
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> OrganisationDetails()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var viewModel = MapOrganisationModel(organisation);

                var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                var userPrivileges = await _contactsApiClient.GetContactPrivileges(Guid.Parse(userId));
                
                viewModel.UserHasChangeOrganisationPrivilege =
                    userPrivileges.Any(cp => cp.Privilege.Key == Privileges.ChangeOrganisationPrivilege);

                if(viewModel.UserHasChangeOrganisationPrivilege == false)
                {
                    var changeOrganisationPriviledge = (await _contactsApiClient.GetPrivileges()).First(p => p.Key == Privileges.ChangeOrganisationPrivilege);
                    viewModel.AccessDeniedViewModel = new AccessDeniedViewModel
                    {
                        Title = Privileges.ChangeOrganisationPrivilege,
                        Description = changeOrganisationPriviledge.Description,
                        PrivilegeId = changeOrganisationPriviledge.Id,
                        ContactId = Guid.Parse(userId),
                        UserHasUserManagement = userPrivileges.Any(up => up.Privilege.Key == Privileges.ManageUsers),
                        ReturnController = ControllerContext.ActionDescriptor.ControllerName,
                        ReturnAction = ControllerContext.ActionDescriptor.ActionName
                    };
                }

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> SelectOrChangeContactName()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var contacts = await _contactsApiClient.GetAllContactsForOrganisation(organisation.OrganisationId);

                var viewModel = new SelectOrChangeContactNameViewModel
                {
                    Contacts = contacts,
                    PrimaryContact = ModelState.IsValid
                        ? organisation.PrimaryContact
                        : ModelState["PrimaryContact"].AttemptedValue
                };
           
                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> SelectOrChangeContactName(SelectOrChangeContactNameViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var contacts = await _contactsApiClient.GetAllContactsForOrganisation(organisation.OrganisationId);

                var primaryContact = !string.IsNullOrEmpty(vm.PrimaryContact)
                    ? await _contactsApiClient.GetByUsername(vm.PrimaryContact)
                    : null;

                // are then any API validations which can be applied instead - should there be??

                if (vm.ActionChoice == "Save")
                {
                    if (string.IsNullOrEmpty(vm.PrimaryContact))
                    {
                        ModelState.AddModelError("PrimaryContact", "Unable to remove the primary contact or save no value");
                    }

                    if (organisation.PrimaryContact == vm.PrimaryContact)
                    {
                        ModelState.AddModelError("PrimaryContact", "Unable to update the selected contact is the same as the current contact");
                    }

                    if (organisation.Id != primaryContact?.OrganisationId)
                    {
                        ModelState.AddModelError("PrimaryContact", "Unable to update the selected contact is not a contact for the organisation");
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("SelectOrChangeContactName");
                    }

                    vm = new SelectOrChangeContactNameViewModel
                    {
                        Contacts = contacts,
                        PrimaryContact = vm.PrimaryContact,
                        PrimaryContactName = contacts.FirstOrDefault(p => p.Username == vm.PrimaryContact)?.DisplayName
                    };

                    return View("SelectOrChangeContactNameConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var request = new AssociateEpaOrganisationWithEpaContactRequest
                    {
                        ContactId = primaryContact.Id,
                        OrganisationId = organisation.OrganisationId,
                        ContactStatus = ContactStatus.Live,
                        MakePrimaryContact = true,
                        AddDefaultRoles = false,
                        AddDefaultPrivileges = false
                    };

                    await _organisationsApiClient.AssociateOrganisationWithEpaContact(request);

                    try
                    {
                        var notifyNewPrimaryContactEmailTemplate = await _emailApiClient.GetEmailTemplate("WHAT_IS_THE_TEMPLATE_NAME");
                        if (notifyNewPrimaryContactEmailTemplate != null)
                        {
                            _logger.LogInformation($"Sending email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name}");

                            await _emailApiClient.SendEmailWithTemplate(new SendEmailRequest(primaryContact.Email, notifyNewPrimaryContactEmailTemplate, new
                            {
                                ServiceName = "Apprenticeship assessment service",
                                Contact = primaryContact.DisplayName,
                                ServiceTeam = "Apprenticeship assessment service team"
                            }));
                        }
                    }
                    catch(Exception)
                    {
                        _logger.LogInformation($"Unable to send email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name}");
                    }

                    var contactsWithPrivileges = await _contactsApiClient.GetContactsWithPrivileges(organisation.Id);
                    var contactsWithManageUserPrivilege = contactsWithPrivileges?
                        .Where(c => c.Privileges.Any(p => p.Key == Privileges.ManageUsers))
                        .Select(p => contacts.First(c => c.Id.Equals(p.Contact.Id)))
                        .ToList();

                    try
                    {
                        var notifyContactsWithManageUsersPermissionEmailTemplate = await _emailApiClient.GetEmailTemplate("WHAT_IS_THE_TEMPLATE_NAME");
                        if (notifyContactsWithManageUsersPermissionEmailTemplate != null)
                        {
                            foreach (var contactWithManageUserPrivilege in contactsWithManageUserPrivilege)
                            {
                                _logger.LogInformation($"Sending email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name} to {contactWithManageUserPrivilege.Username}");

                                await _emailApiClient.SendEmailWithTemplate(new SendEmailRequest(contactWithManageUserPrivilege.Email, notifyContactsWithManageUsersPermissionEmailTemplate, new
                                {
                                    ServiceName = "Apprenticeship assessment service",
                                    Contact = contactWithManageUserPrivilege.DisplayName,
                                    ServiceTeam = "Apprenticeship assessment service team"
                                }));
                            }
                        }
                    }
                    catch(Exception)
                    {
                        _logger.LogInformation($"Unable to send email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name} to contacts with mangage user privileges");
                    }

                    vm = new SelectOrChangeContactNameViewModel
                    {
                        Contacts = contactsWithManageUserPrivilege,
                        PrimaryContact = vm.PrimaryContact,
                        PrimaryContactName = contacts.FirstOrDefault(p => p.Username == vm.PrimaryContact)?.DisplayName
                    };

                    return View("SelectOrChangeContactNameUpdated", vm);
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return RedirectToAction("OrganisationDetails");
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                return RedirectToAction("OrganisationDetails");
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeAddress()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                return RedirectToAction("OrganisationDetails");
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeEmail()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                return RedirectToAction("OrganisationDetails");
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeWebsite()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                return RedirectToAction("OrganisationDetails");
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        private ViewAndEditOrganisationViewModel MapOrganisationModel(EpaOrganisation organisation)
        {
            var viewModel = new ViewAndEditOrganisationViewModel
            {
                OrganisationId = organisation.OrganisationId,
                Email = organisation.OrganisationData?.Email,
                PhoneNumber = organisation.OrganisationData?.PhoneNumber,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = !string.IsNullOrEmpty(organisation.PrimaryContact)
                    ? organisation.PrimaryContact
                    : string.Empty,
                PrimaryContactName = !string.IsNullOrEmpty(organisation.PrimaryContactName)
                    ? organisation.PrimaryContactName
                    : string.Empty
            };

            return viewModel;
        }
    }
}